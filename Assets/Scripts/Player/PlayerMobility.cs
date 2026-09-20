using System.Collections.Generic;
using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>Opt-in movement test pack. Existing player prefabs need not enable it.</summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-60)]
    [RequireComponent(typeof(CapsuleCollider2D), typeof(PlayerJump), typeof(PlayerHealth))]
    public sealed class PlayerMobility : MonoBehaviour
    {
        [SerializeField] private LayerMask _solidLayers = 8;
        [SerializeField, Range(0.5f, 1.2f)] private float _airJumpMultiplier = 1f;
        [SerializeField, Range(0.4f, 0.8f)] private float _crouchHeight = 0.68f;
        [SerializeField, Min(0.1f)] private float _dashSpeed = 13f;
        [SerializeField, Min(0.05f)] private float _dashDuration = 0.18f;
        [SerializeField, Min(0.1f)] private float _dashCooldown = 0.65f;
        [SerializeField, Min(0f)] private float _dodgeDuration = 0.11f;

        public float AirJumpMultiplier => _airJumpMultiplier;
        public bool IsDashing { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsDodging => isActiveAndEnabled && IsDashing &&
            Time.time - _dashStarted < _dodgeDuration;
        public bool IsHurt => isActiveAndEnabled && Time.time < _hurtUntil;
        // Presentation may finish recovering after the gameplay interruption ends.
        private const float HurtVisualDuration = 0.42f;
        public bool IsHurtVisual => isActiveAndEnabled && Time.time < _hurtVisualUntil;
        public float HurtProgress => Mathf.Clamp01(1f - (_hurtVisualUntil - Time.time) / HurtVisualDuration);
        public bool BlocksAttack => IsDashing || IsCrouching || IsHurt;
        public int DashDirection { get; private set; } = 1;
        public float DashVelocity { get; private set; }

        private PlayerInputReader _input;
        private PlayerJump _jump;
        private PlayerAttack _attack;
        private PlayerHealth _health;
        private CapsuleCollider2D _capsule;
        private Rigidbody2D _body;
        private Vector2 _standingSize, _standingOffset;
        private bool _dashPressed;
        private bool _airDashAvailable = true;
        private float _preDashGravity, _preDashVerticalSpeed;
        private float _dashStarted, _nextDash, _hurtUntil;
        private float _hurtVisualUntil;
        private ContactFilter2D _solids;
        private readonly List<Collider2D> _overlaps = new(8);
        private readonly List<RaycastHit2D> _hits = new(8);

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();
            _jump = GetComponent<PlayerJump>();
            _attack = GetComponent<PlayerAttack>();
            _health = GetComponent<PlayerHealth>();
            _capsule = GetComponent<CapsuleCollider2D>();
            _body = GetComponent<Rigidbody2D>();
            _standingSize = _capsule.size;
            _standingOffset = _capsule.offset;
            _solids = new ContactFilter2D();
            _solids.SetLayerMask(_solidLayers);
            _solids.useTriggers = false;
        }

        private void OnEnable() { _health.Damaged += OnDamaged; }
        private void Update()
        {
            if (!_input.IsGameplayBlocked && _input.DashPressedThisFrame) _dashPressed = true;
        }

        private void FixedUpdate()
        {
            bool requested = _dashPressed; _dashPressed = false;
            if (_input.IsGameplayBlocked || _health.IsDead || !_body.simulated)
            { EndDash(); return; }
            if (_jump.IsGrounded) _airDashAvailable = true;
            if (IsDashing && !_jump.IsGrounded) _airDashAvailable = false;
            if (IsDashing && Time.time - _dashStarted >= _dashDuration)
                EndDash();
            bool wantsCrouch = _input.CrouchHeld && _jump.IsGrounded &&
                !IsDashing && !_attack.IsAttacking;
            if (wantsCrouch) SetCrouch(true);
            else if (IsCrouching && HasStandingRoom()) SetCrouch(false);

            if (requested && !IsDashing && !IsCrouching && !IsHurt &&
                (_jump.IsGrounded || _airDashAvailable) &&
                !_attack.IsAttacking && Time.time >= _nextDash)
            {
                IsDashing = true; _dashStarted = Time.time; _nextDash = Time.time + _dashCooldown;
                if (!_jump.IsGrounded) _airDashAvailable = false;
                _preDashGravity = _body.gravityScale;
                _preDashVerticalSpeed = _body.linearVelocity.y;
                _body.gravityScale = 0f;
                _body.linearVelocity = new Vector2(_body.linearVelocity.x, 0f);
                DashDirection = Mathf.Abs(_input.Horizontal) > 0.1f
                    ? (_input.Horizontal < 0f ? -1 : 1) : _attack.FacingDirection;
            }
            if (!IsDashing) return;
            // Sweep before assigning velocity: no position teleport or wall tunnelling.
            float distance = _dashSpeed * Time.fixedDeltaTime;
            int count = _capsule.Cast(Vector2.right * DashDirection, _solids, _hits, distance + 0.02f);
            for (int i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (hit.collider == null || hit.collider.attachedRigidbody == _body ||
                    Vector2.Dot(hit.normal, Vector2.right * DashDirection) > -0.5f) continue;
                distance = Mathf.Min(distance, Mathf.Max(0f, hit.distance - 0.02f));
            }
            DashVelocity = DashDirection * distance / Time.fixedDeltaTime;
            if (distance < 0.001f) EndDash();
        }

        public bool TryStandForJump()
        {
            if (!IsCrouching) return true;
            if (!HasStandingRoom()) return false;
            SetCrouch(false);
            return true;
        }

        private bool HasStandingRoom()
        {
            // Check only the space added ABOVE the current crouched collider.
            // Inset avoids counting the floor as an obstruction on expansion.
            float bottom = _standingOffset.y - _standingSize.y * 0.5f;
            float low = bottom + _standingSize.y * _crouchHeight;
            float high = _standingOffset.y + _standingSize.y * 0.5f;
            Vector2 center = transform.TransformPoint(new Vector3(_standingOffset.x, (low + high) * 0.5f, 0f));
            Vector3 scale = transform.lossyScale;
            Vector2 size = new Vector2(Mathf.Abs(scale.x) * _standingSize.x - 0.04f,
                Mathf.Abs(scale.y) * (high - low) - 0.04f);
            int count = Physics2D.OverlapBox(center, size, transform.eulerAngles.z, _solids, _overlaps);
            for (int i = 0; i < count; i++)
                if (_overlaps[i] != null && _overlaps[i].attachedRigidbody != _body) return false;
            return true;
        }

        private void SetCrouch(bool crouch)
        {
            IsCrouching = crouch;
            float height = crouch ? _standingSize.y * _crouchHeight : _standingSize.y;
            _capsule.size = new Vector2(_standingSize.x, height);
            _capsule.offset = _standingOffset + Vector2.down * ((_standingSize.y - height) * 0.5f);
        }

        private void OnDamaged()
        {
            EndDash();
            _hurtUntil = _health.IsDead ? 0f : Time.time + 0.24f;
            _hurtVisualUntil = _health.IsDead ? 0f : Time.time + HurtVisualDuration;
            _attack.ResetTransientState();
        }

        private void EndDash()
        {
            if (IsDashing && _body != null)
            {
                _body.gravityScale = _preDashGravity;
                // Dash cancels ascent rather than storing a second launch. Falling
                // momentum resumes, so repeated inputs cannot suspend the player.
                _body.linearVelocity = new Vector2(_body.linearVelocity.x,
                    Mathf.Min(0f, _preDashVerticalSpeed));
            }
            IsDashing = false;
            DashVelocity = 0f;
        }

        public void ResetTransientState(bool restoreStanding)
        {
            EndDash(); _dashPressed = false; _nextDash = 0f; _hurtUntil = 0f;
            _hurtVisualUntil = 0f;
            _airDashAvailable = true;
            if (restoreStanding) SetCrouch(false);
        }

        private void OnDisable()
        {
            if (_health != null) _health.Damaged -= OnDamaged;
            ResetTransientState(true);
        }
    }
}

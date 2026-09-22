using System.Collections.Generic;
using KitchenChaos.Input;
using KitchenChaos.Enemy;
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
        public bool IsParried => isActiveAndEnabled && Time.time < _parriedUntil;
        public float ParryVelocity { get; private set; }
        public const float ParryStunDuration = .45f;
        private const float ParryTravelDuration = .22f;
        public bool IsHurt => isActiveAndEnabled && (Time.time < _hurtUntil || IsParried);
        // Presentation may finish recovering after the gameplay interruption ends.
        private const float HurtVisualDuration = 0.42f;
        public bool IsHurtVisual => isActiveAndEnabled && (Time.time < _hurtVisualUntil || IsParried);
        public float HurtProgress => IsParried ? Mathf.Clamp01(1f - (_parriedUntil - Time.time) / ParryStunDuration) :
            Mathf.Clamp01(1f - (_hurtVisualUntil - Time.time) / HurtVisualDuration);
        public bool BlocksAttack => IsDashing || IsCrouching || IsHurt;
        public int DashDirection { get; private set; } = 1;
        public float DashVelocity { get; private set; }

        private PlayerInputReader _input;
        private PlayerJump _jump;
        private PlayerAttack _attack;
        private PlayerSpinAttack _spin;
        private PlayerHealth _health;
        private CapsuleCollider2D _capsule;
        private Rigidbody2D _body;
        private Vector2 _standingSize, _standingOffset;
        private bool _dashPressed;
        private bool _groundDash, _dashBlockedByEnemy;
        private bool _airDashAvailable = true;
        private float _preDashGravity, _preDashVerticalSpeed;
        private float _dashStarted, _nextDash, _hurtUntil;
        private float _hurtVisualUntil;
        private float _parriedUntil, _parryMoveUntil, _parryDistance, _parrySpeed, _parryFloorY;
        private int _parryDirection;
        private bool _parryHasGround;
        private readonly List<RaycastHit2D> _parryGroundHits = new(8);
        private ContactFilter2D _solids;
        private ContactFilter2D _dashEnemyFilter;
        private readonly List<Collider2D> _overlaps = new(8);
        private readonly List<RaycastHit2D> _hits = new(8);

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();
            _jump = GetComponent<PlayerJump>();
            _attack = GetComponent<PlayerAttack>();
            _spin = GetComponent<PlayerSpinAttack>();
            _health = GetComponent<PlayerHealth>();
            _capsule = GetComponent<CapsuleCollider2D>();
            _body = GetComponent<Rigidbody2D>();
            _standingSize = _capsule.size;
            _standingOffset = _capsule.offset;
            _solids = new ContactFilter2D();
            _solids.SetLayerMask(_solidLayers);
            _solids.useTriggers = false;
            _dashEnemyFilter = new ContactFilter2D { useTriggers = true };
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
            { EndDash(); ClearParry(); return; }
            if (IsParried) { UpdateParryMotion(); return; }
            ParryVelocity = 0f;
            if (_jump.IsGrounded) _airDashAvailable = true;
            if (IsDashing && !_jump.IsGrounded) _airDashAvailable = false;
            if (IsDashing && Time.time - _dashStarted >= _dashDuration)
                EndDash();
            bool wantsCrouch = _input.CrouchHeld && _jump.IsGrounded &&
                !IsDashing && !_attack.IsAttacking && !(_spin != null && _spin.IsSpinning);
            // Keep the collider/pose chosen at melee start even if crouch is released.
            if (!_attack.IsAttacking)
            {
                if (wantsCrouch) SetCrouch(true);
                else if (IsCrouching && HasStandingRoom()) SetCrouch(false);
            }

            if (requested && !IsDashing && !IsCrouching && !IsHurt &&
                (_jump.IsGrounded || _airDashAvailable) &&
                !_attack.IsAttacking && Time.time >= _nextDash)
            {
                IsDashing = true; _dashStarted = Time.time; _nextDash = Time.time + _dashCooldown;
                _groundDash = _jump.IsGrounded;
                _dashBlockedByEnemy = false;
                if (!_jump.IsGrounded) _airDashAvailable = false;
                _preDashGravity = _body.gravityScale;
                _preDashVerticalSpeed = _body.linearVelocity.y;
                _body.gravityScale = 0f;
                _body.linearVelocity = new Vector2(_body.linearVelocity.x, 0f);
                DashDirection = Mathf.Abs(_input.Horizontal) > 0.1f
                    ? (_input.Horizontal < 0f ? -1 : 1) : _attack.FacingDirection;
            }
            if (!IsDashing) return;
            // Keep the remaining dash window at zero speed after an enemy stop.
            // Ending immediately would let normal movement overwrite the stop
            // in PlayerMovement later in this same physics tick.
            if (_dashBlockedByEnemy) { DashVelocity = 0f; return; }
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
            if (_groundDash) distance = LimitGroundDashAtEnemies(distance);
            DashVelocity = DashDirection * distance / Time.fixedDeltaTime;
            if (distance < 0.001f && !_dashBlockedByEnemy) EndDash();
        }

        private float LimitGroundDashAtEnemies(float distance)
        {
            Vector2 direction = Vector2.right * DashDirection;
            // Explicit overlap covers a dash started already touching/inside a
            // trigger hurtbox, independent of queriesStartInColliders.
            Vector2 size = Vector2.Scale(_capsule.size,
                new Vector2(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y)));
            int count = Physics2D.OverlapCapsule(transform.TransformPoint(_capsule.offset), size,
                _capsule.direction, transform.eulerAngles.z, _dashEnemyFilter, _overlaps);
            for (int i = 0; i < count; i++)
                if (IsEnemyAhead(_overlaps[i]))
                { _dashBlockedByEnemy = true; return 0f; }
            count = _capsule.Cast(direction, _dashEnemyFilter, _hits, distance + .04f);
            for (int i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (!IsEnemyAhead(hit.collider)) continue;
                distance = Mathf.Min(distance, Mathf.Max(0f, hit.distance - .04f));
                _dashBlockedByEnemy = true;
            }
            return distance;
        }

        private bool IsEnemyAhead(Collider2D other)
        {
            if (other == null || other.attachedRigidbody == _body ||
                (other.bounds.center.x - _capsule.bounds.center.x) * DashDirection < -.01f)
                return false; // Moving away from an overlapping enemy stays possible.
            var enemy = other.GetComponentInParent<EnemyHealth>();
            return enemy != null && enemy.isActiveAndEnabled && enemy.CurrentHealth > 0;
        }

        public bool TryStandForJump()
        {
            if (IsParried) return false;
            if (_attack.IsAttacking && _attack.IsCrouchedSwing) return false;
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
            ClearParry();
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
            _groundDash = _dashBlockedByEnemy = false;
            DashVelocity = 0f;
        }

        public void ResetTransientState(bool restoreStanding)
        {
            ClearParry();
            EndDash(); _dashPressed = false; _nextDash = 0f; _hurtUntil = 0f;
            _hurtVisualUntil = 0f;
            _airDashAvailable = true;
            if (restoreStanding) SetCrouch(false);
        }

        /// <summary>Non-damaging shield rejection. Ordinary health/invulnerability is untouched.</summary>
        public bool ReceiveShieldParry(Vector2 source, float safeSeparation)
        {
            if (!isActiveAndEnabled || IsParried || _health.IsDead || _input.IsGameplayBlocked || !_body.simulated)
                return false;
            EndDash();
            _parryDirection = transform.position.x < source.x ? -1 : 1;
            _parryDistance = Mathf.Clamp(safeSeparation - Mathf.Abs(transform.position.x - source.x), 1.5f, 3.2f);
            _parrySpeed = _parryDistance / ParryTravelDuration;
            _parriedUntil = Time.time + ParryStunDuration;
            _parryMoveUntil = Time.time + ParryTravelDuration;
            _dashPressed = false;
            _attack.ResetTransientState();
            // Keep facing and crouched collision shape; no forced standing under ceilings.
            Bounds b = _capsule.bounds;
            int count = Physics2D.Raycast(new Vector2(b.center.x, b.min.y + .1f), Vector2.down,
                _solids, _parryGroundHits, 6f);
            _parryHasGround = false;
            float closest = float.PositiveInfinity;
            for (int i = 0; i < count; i++)
                if (_parryGroundHits[i].collider != null && _parryGroundHits[i].collider.attachedRigidbody != _body &&
                    _parryGroundHits[i].distance < closest)
                {
                    closest = _parryGroundHits[i].distance;
                    _parryFloorY = _parryGroundHits[i].point.y;
                    _parryHasGround = true;
                }
            float vertical = _body.linearVelocity.y;
            if (_jump.IsGrounded && _parryHasGround && SafeParryStep(.12f) >= .1f) vertical = 3f;
            UpdateParryMotion();
            // The melee query runs after movement, so replace its already-written speed now.
            _body.linearVelocity = new Vector2(ParryVelocity, vertical);
            return true;
        }

        private void UpdateParryMotion()
        {
            ParryVelocity = 0f;
            if (Time.time >= _parryMoveUntil || _parryDistance <= 0f) return;
            float wanted = Mathf.Min(_parrySpeed * Time.fixedDeltaTime, _parryDistance);
            float safe = SafeParryStep(wanted);
            ParryVelocity = _parryDirection * safe / Time.fixedDeltaTime;
            _parryDistance = safe + .001f < wanted ? 0f : Mathf.Max(0f, _parryDistance - safe);
        }

        private float SafeParryStep(float distance)
        {
            if (!_parryHasGround) return 0f; // Never add horizontal recoil over an unknown drop.
            Vector2 direction = Vector2.right * _parryDirection;
            int count = _capsule.Cast(direction, _solids, _hits, distance + .04f);
            for (int i = 0; i < count; i++)
                if (_hits[i].collider != null && _hits[i].collider.attachedRigidbody != _body &&
                    Vector2.Dot(_hits[i].normal, direction) < -.5f)
                    distance = Mathf.Min(distance, Mathf.Max(0f, _hits[i].distance - .04f));
            Bounds b = _capsule.bounds;
            // Check every leading-foot sample at the original support level, even during the hop.
            for (float sample = 0f; sample <= distance + .049f; sample += .05f)
            {
                float offset = Mathf.Min(sample, distance);
                Vector2 foot = new(b.center.x + _parryDirection * (b.extents.x + offset + .06f), _parryFloorY + .15f);
                if (Physics2D.Raycast(foot, Vector2.down, _solids, _parryGroundHits, .3f) == 0)
                    return Mathf.Max(0f, offset - .05f);
            }
            return distance;
        }

        private void ClearParry()
        {
            _parriedUntil = _parryMoveUntil = _parryDistance = 0f;
            ParryVelocity = 0f;
        }

        private void OnDisable()
        {
            if (_health != null) _health.Damaged -= OnDamaged;
            ResetTransientState(true);
        }
    }
}

using System;
using System.Collections.Generic;
using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>Opt-in fork attack. Melee (-40) wins simultaneous input; dash cancels windup.</summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(70)]
    [RequireComponent(typeof(PlayerAttack), typeof(PlayerHealth), typeof(PlayerRespawn))]
    public sealed class PlayerRangedAttack : MonoBehaviour
    {
        [SerializeField] private ForkProjectile _projectilePrefab;
        [SerializeField] private SpriteRenderer _heldFork;
        [SerializeField] private SpriteRenderer _chefRenderer;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip _throwSound;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField, Min(0.1f)] private float _shotInterval = 0.6f;
        [SerializeField, Min(0.02f)] private float _windup = 0.12f;
        [SerializeField, Min(0.02f)] private float _release = 0.08f;
        [SerializeField, Min(0.02f)] private float _recovery = 0.15f;
        [SerializeField] private LayerMask _solidLayers = 8;
        [SerializeField] private bool _allowDownwardAim;
        public bool IsThrowing => isActiveAndEnabled && _throwing;
        public int ThrowFacing { get; private set; } = 1;
        public Vector2 ShotDirection { get; private set; } = Vector2.right;
        public int PoseIndex => !_fired ? 0 : Time.time - _started < _windup + _release ? 1 : 2;
        public event Action<Vector2> HitConnected;
        private PlayerInputReader _input;
        private PlayerAttack _melee;
        private PlayerSpinAttack _spin;
        private PlayerMobility _mobility;
        private PlayerHealth _health;
        private PlayerRespawn _respawn;
        private ChefLocomotionVisual _visual;
        private CapsuleCollider2D _capsule;
        private bool _throwing, _fired, _ready;
        private float _started, _nextShot;
        private readonly HashSet<ForkProjectile> _shots = new();
        private readonly List<ForkProjectile> _clearBuffer = new();

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>(); _melee = GetComponent<PlayerAttack>();
            _spin = GetComponent<PlayerSpinAttack>();
            _mobility = GetComponent<PlayerMobility>(); _health = GetComponent<PlayerHealth>();
            _respawn = GetComponent<PlayerRespawn>(); _visual = GetComponent<ChefLocomotionVisual>();
            _capsule = GetComponent<CapsuleCollider2D>();
            _ready = _input != null && _mobility != null && _visual != null && _capsule != null &&
                _projectilePrefab != null && _heldFork != null && _chefRenderer != null && _audio != null;
            if (!_ready) { Debug.LogError("PlayerRangedAttack needs its fork, visual and audio references.", this); enabled = false; }
        }

        private void OnEnable()
        {
            if (!_ready) return;
            _health.Damaged += CancelThrow;
            _health.Died += ResetCombat;
            _respawn.Respawning += ResetCombat;
        }

        private void FixedUpdate()
        {
            if (!_ready) return;
            if (_health.IsDead || _input.IsGameplayBlocked || !_respawn.CanInteract ||
                _mobility.IsDashing || _mobility.IsHurt || _melee.IsAttacking || (_spin != null && _spin.IsSpinning))
            { CancelThrow(); return; }
            if (_throwing)
            {
                if (!_fired && Time.time - _started >= _windup)
                {
                    _fired = true;
                    Fire();
                }
                if (Time.time - _started >= _windup + _release + _recovery) CancelThrow();
                return;
            }
            if (!_input.RangedHeld || Time.time < _nextShot) return;
            ThrowFacing = _melee.FacingDirection;
            // Lock one direction per throw so the release pose and projectile agree.
            ShotDirection = QuantizeAim(_input.Aim, ThrowFacing, _allowDownwardAim);
            _throwing = true; _fired = false; _started = Time.time;
            _nextShot = Time.time + _shotInterval;
        }

        private static Vector2 QuantizeAim(Vector2 aim, int facing, bool allowDownward)
        {
            if (allowDownward && aim.y <= -0.45f)
                return Mathf.Abs(aim.x) >= 0.35f ? new Vector2(facing, -1f).normalized : Vector2.down;
            if (aim.y < 0.45f) return Vector2.right * facing;
            return Mathf.Abs(aim.x) >= 0.35f ? new Vector2(facing, 1f).normalized : Vector2.up;
        }

        private void Fire()
        {
            // Use the authored RELEASE hand, not last render frame's windup fist.
            Vector3 hand;
            if (!_visual.TryGetThrowReleaseWorldPosition(_mobility.IsCrouching, ShotDirection, ThrowFacing, out hand))
                hand = transform.position;
            // Start inside the body, even when the authored hand is above a low ceiling.
            Vector2 origin = _capsule.bounds.center;
            // Sweep chest-to-hand on launch as well: a hand drawn across a wall
            // must not create a projectile on its far side.
            var shot = Instantiate(_projectilePrefab, origin, Quaternion.identity);
            _shots.Add(shot);
            shot.Initialize(this, ShotDirection, _solidLayers, origin, hand);
            if (_throwSound != null) { _audio.pitch = 1.25f; _audio.PlayOneShot(_throwSound, 0.45f); }
        }

        private void LateUpdate()
        {
            if (!_ready) return;
            bool show = IsThrowing && !_fired && !_health.IsDead && !_mobility.IsDashing && !_mobility.IsHurt;
            _heldFork.enabled = show;
            if (!show) return;
            Vector3 hand;
            if (!_visual.TryGetGripWorldPosition(out hand)) return;
            // The drawn backswing fist holds the fork upward, before the release
            // wrist follows the selected target direction.
            Vector2 heldDirection = new Vector2(0.34f * ThrowFacing, 0.94f).normalized;
            float angle = Mathf.Atan2(heldDirection.y, heldDirection.x) * Mathf.Rad2Deg;
            _heldFork.transform.SetPositionAndRotation(hand + (Vector3)heldDirection,
                Quaternion.Euler(0f, 0f, angle));
            _heldFork.color = _chefRenderer.color;
        }

        public void ReportHit(Vector2 point)
        {
            if (_hitSound != null && _audio != null) { _audio.pitch = 1.35f; _audio.PlayOneShot(_hitSound, 0.35f); }
            HitConnected?.Invoke(point);
        }
        public void Forget(ForkProjectile shot) => _shots.Remove(shot);
        private void CancelThrow()
        {
            _throwing = false; _fired = false;
            if (_heldFork != null) _heldFork.enabled = false;
        }
        private void ResetCombat()
        {
            CancelThrow(); _nextShot = 0f;
            _clearBuffer.Clear(); _clearBuffer.AddRange(_shots);
            foreach (var shot in _clearBuffer) if (shot != null) shot.Despawn();
            _clearBuffer.Clear(); _shots.Clear();
        }
        private void OnDisable()
        {
            if (_health != null) { _health.Damaged -= CancelThrow; _health.Died -= ResetCombat; }
            if (_respawn != null) _respawn.Respawning -= ResetCombat;
            ResetCombat();
        }
    }
}

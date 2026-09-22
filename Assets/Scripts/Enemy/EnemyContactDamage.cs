using KitchenChaos.Player;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Damages the player while it stays in contact with this enemy.
    /// Kept separate from <see cref="EnemyPatrol"/> so a stationary hazard can reuse
    /// the same contact damage rules without inheriting any movement.
    /// </summary>
    [DefaultExecutionOrder(-20)]
    public sealed class EnemyContactDamage : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _damage = 1;
        [SerializeField, Min(0f)] private float _damageInterval = 1f;
        [Header("Optional telegraphed tomato punch")]
        [SerializeField] private bool _useAttackAnimation;
        [SerializeField, Min(0.02f)] private float _windup = 0.18f;
        [SerializeField, Min(0.02f)] private float _activeTime = 0.12f;
        [SerializeField, Min(0.02f)] private float _recovery = 0.20f;
        [SerializeField, Min(0f)] private float _punchReach = 0.55f;
        [Header("Optional gap-closing lunge")]
        [SerializeField] private bool _useLunge;
        [SerializeField, Min(1f)] private float _detectionRange = 6f;
        [SerializeField, Min(0.1f)] private float _lungeWindup = 0.32f;
        [SerializeField, Min(1f)] private float _lungeSpeed = 13f;
        [SerializeField, Min(0.1f)] private float _lungeDistance = 4.5f;
        [SerializeField] private LayerMask _solidLayers = 8;
        [SerializeField, Min(0.1f)] private float _chaseSpeed = 3.4f;
        public bool ControlsMovement => isActiveAndEnabled && _useLunge && (IsAttacking || _target != null);
        public bool IsPreparingLunge => IsAttacking && AttackPose == 0 && _lungingAttack;
        private bool _openingUsed;
        private Collider2D _target;
        private Rigidbody2D _body;
        private EnemySpinKnockback _knockback;
        private Camera _camera;
        private ContactFilter2D _solids;
        private readonly List<RaycastHit2D> _casts = new(8);
        private float _remainingLunge;
        private bool _lungingAttack;
        public int AttackPose { get; private set; } = -1;
        public int AttackFacing { get; private set; } = 1;
        public bool IsAttacking => isActiveAndEnabled && _useAttackAnimation && AttackPose >= 0;
        private Collider2D _collider;
        private EnemyHealth _health;
        private float _phaseEnds;
        private bool _hitThisAttack;
        private readonly List<Collider2D> _overlaps = new(8);
        private ContactFilter2D _filter;

        private float _nextDamageTime;
        private EnemyHitStun _hitStun;

        private void Awake()
        {
            _hitStun = GetComponent<EnemyHitStun>();
            _health = GetComponent<EnemyHealth>();
            _collider = GetComponent<Collider2D>();
            _body = GetComponent<Rigidbody2D>();
            _knockback = GetComponent<EnemySpinKnockback>();
            _camera = Camera.main;
            _solids = new ContactFilter2D { useTriggers = false };
            _solids.SetLayerMask(_solidLayers);
            _filter = new ContactFilter2D { useTriggers = false };

            // Without a trigger collider the enemy is silently harmless, so fail once
            // and loudly instead of leaving the designer to guess.
            if (!TryGetComponent(out Collider2D damageCollider) || !damageCollider.isTrigger)
            {
                Debug.LogError($"{nameof(EnemyContactDamage)} needs a Collider2D with Is Trigger enabled.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            ResetEncounter();
            if (_health != null)
            {
                _health.Restored += ResetEncounter;
                _health.Damaged += AlertOnDamage;
            }
            if (_hitStun != null) _hitStun.StunStarted += CancelForStun;
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.Restored -= ResetEncounter;
                _health.Damaged -= AlertOnDamage;
            }
            if (_hitStun != null) _hitStun.StunStarted -= CancelForStun;
            ResetAttack();
        }

        private void ResetEncounter()
        {
            ResetAttack();
            _openingUsed = false;
            _target = null;
        }

        private void AlertOnDamage()
        {
            if (!_useAttackAnimation || !_useLunge || _health.CurrentHealth <= 0 || _target != null) return;
            // All current player weapons damage these enemies; taking a hit is
            // awareness, not stun. A fork must also interrupt unaware patrol.
            var player = FindFirstObjectByType<PlayerHealth>();
            if (player == null || !player.isActiveAndEnabled || player.IsDead) return;
            _target = player.GetComponent<Collider2D>();
            if (_target == null) return;
            if (!IsAttacking)
                AttackFacing = _target.bounds.center.x < _collider.bounds.center.x ? -1 : 1;
            // Preserve an attack already committed, its cooldown and the one-use
            // opening. Normal update still enforces screen, wall and pit safety.
        }

        private void ResetAttack()
        {
            AttackPose = -1;
            _nextDamageTime = 0f;
            _hitThisAttack = false;
            _remainingLunge = 0f;
            _lungingAttack = false;
        }

        private void CancelForStun()
        {
            if (!_useLunge) return;
            // Once the stun ends, allow a fresh readable counterattack instead of
            // adding the cancelled attack's old cooldown to the stun duration.
            ResetAttack();
        }

        private void FixedUpdate()
        {
            if (!_useAttackAnimation) return;
            if (_useLunge && _knockback != null && _knockback.IsPushed)
            {
                AttackPose = -1;
                _remainingLunge = 0f;
                _nextDamageTime = Mathf.Max(_nextDamageTime, Time.time + 0.2f);
                return;
            }
            if ((_health != null && _health.CurrentHealth <= 0) ||
                (_hitStun != null && _hitStun.IsStunned))
            {
                // A cancelled windup never resumes directly on its damaging frame.
                AttackPose = -1;
                _remainingLunge = 0f;
                if (_useLunge && _body != null)
                {
                    _body.linearVelocity = Vector2.zero;
                    _body.MovePosition(_body.position);
                }
                return;
            }
            if (ControlsMovement && _body != null)
            {
                _body.linearVelocity = Vector2.zero;
                _body.MovePosition(_body.position);
            }
            if (AttackPose < 0)
            {
                Bounds b = _collider.bounds;
                if (_useLunge && _target != null)
                {
                    var health = _target.GetComponentInParent<PlayerHealth>();
                    if (!_target.isActiveAndEnabled || health == null || health.IsDead ||
                        Mathf.Abs(_target.bounds.center.x - b.center.x) > _detectionRange + 4f)
                        _target = null; // Losing sight never replenishes the opening.
                }
                if (_useLunge && !OnScreen(b.center)) return;
                if (_useLunge && _target != null)
                {
                    Vector2 aim = _target.ClosestPoint(b.center);
                    AttackFacing = _target.bounds.center.x < b.center.x ? -1 : 1;
                    bool sameHeight = _target.bounds.min.y < b.max.y && _target.bounds.max.y > b.min.y;
                    bool clear = !Physics2D.Linecast(b.center, aim, _solidLayers);
                    float gap = Mathf.Abs(aim.x - b.center.x);
                    if (!_openingUsed && sameHeight && clear && OpeningCanReach(b, gap) &&
                        Time.time >= _nextDamageTime)
                    {
                        bool lunge = gap > b.extents.x + _punchReach;
                        float openingDistance = Mathf.Min(_lungeDistance,
                            Mathf.Max(0f, gap - b.extents.x - .15f));
                        if (!lunge || SafeLungeStep(openingDistance) + .03f >= openingDistance)
                        {
                            _openingUsed = true;
                            BeginAttack(lunge, openingDistance);
                            return;
                        }
                    }
                    if (sameHeight && clear && Mathf.Abs(aim.x - b.center.x) <= b.extents.x + _punchReach)
                    {
                        if (Time.time >= _nextDamageTime) BeginAttack(false, 0f);
                    }
                    else if (_body != null)
                    {
                        float distance = Mathf.Min(_chaseSpeed * Time.fixedDeltaTime,
                            Mathf.Max(0f, Mathf.Abs(_target.bounds.center.x - b.center.x) - b.extents.x - 0.2f));
                        _body.MovePosition(_body.position + Vector2.right * (AttackFacing * SafeLungeStep(distance)));
                    }
                    return;
                }
                if (Time.time < _nextDamageTime) return;
                int count = Physics2D.OverlapBox(b.center,
                    new Vector2(_useLunge ? _detectionRange * 2f : b.size.x + _punchReach * 2f,
                        b.size.y), 0f, _filter, _overlaps);
                for (int i = 0; i < count; i++)
                {
                    var player = _overlaps[i].GetComponentInParent<PlayerHealth>();
                    if (player == null || !player.isActiveAndEnabled || player.IsDead) continue;
                    Vector2 aim = _overlaps[i].ClosestPoint(b.center);
                    if (_useLunge && Physics2D.Linecast(b.center, aim, _solidLayers)) continue;
                    AttackFacing = player.transform.position.x < b.center.x ? -1 : 1;
                    float gap = Mathf.Abs(aim.x - b.center.x);
                    bool wantsLunge = _useLunge && gap > b.extents.x + _punchReach;
                    // Detection acquires the target; it is not permission to spend
                    // the opening dash before its finite travel can reach them.
                    if (wantsLunge && !_openingUsed && !OpeningCanReach(b, gap))
                    {
                        _target = _overlaps[i];
                        break; // Approach next step without consuming the opening.
                    }
                    float openingDistance = Mathf.Min(_lungeDistance,
                        Mathf.Max(0f, gap - b.extents.x - 0.15f));
                    // Eye-level sight alone misses low crates. Check the whole
                    // body/foot path before signalling or spending the opening.
                    if (wantsLunge && !_openingUsed &&
                        SafeLungeStep(openingDistance) + 0.03f < openingDistance)
                        continue;
                    if (_useLunge)
                    {
                        _target = _overlaps[i];
                        if (_openingUsed) break; // Reacquired targets are pursued next step.
                        _openingUsed = true; // Consumed on windup, not on a successful hit.
                    }
                    BeginAttack(wantsLunge, openingDistance);
                    break;
                }
                return;
            }
            if (Time.time >= _phaseEnds)
            {
                AttackPose++;
                if (AttackPose > 2) { AttackPose = -1; return; }
                _phaseEnds = Time.time + (AttackPose == 1
                    ? _activeTime + (_lungingAttack ? _remainingLunge / _lungeSpeed : 0f) : _recovery);
            }
            if (AttackPose != 1) return;
            float step = 0f;
            if (_remainingLunge > 0f && _body != null)
            {
                float requested = Mathf.Min(_remainingLunge, _lungeSpeed * Time.fixedDeltaTime);
                step = SafeLungeStep(requested);
                _body.MovePosition(_body.position + Vector2.right * (AttackFacing * step));
                _remainingLunge = step + 0.001f < requested ? 0f : Mathf.Max(0f, _remainingLunge - step);
                if (_remainingLunge <= 0f) _phaseEnds = Time.time + _activeTime;
            }
            if (_hitThisAttack) return;
            Bounds bounds = _collider.bounds;
            // Sweep the punch over this physics step, not only its starting point.
            float reach = bounds.extents.x + _punchReach + step;
            Vector2 center = (Vector2)bounds.center + Vector2.right * (AttackFacing * reach * 0.5f);
            int hits = Physics2D.OverlapBox(center, new Vector2(reach, bounds.size.y),
                0f, _filter, _overlaps);
            for (int i = 0; i < hits; i++)
            {
                var player = _overlaps[i].GetComponentInParent<PlayerHealth>();
                if (player == null || !player.isActiveAndEnabled || player.IsDead) continue;
                if (_useLunge && Physics2D.Linecast(bounds.center,
                    _overlaps[i].ClosestPoint(bounds.center), _solidLayers)) continue;
                _hitThisAttack = true;
                player.TakeDamage(_damage);
                break;
            }
        }

        private void BeginAttack(bool lunge, float distance)
        {
            _lungingAttack = lunge;
            _remainingLunge = lunge ? distance : 0f;
            AttackPose = 0;
            _phaseEnds = Time.time + (lunge ? _lungeWindup : _windup);
            _nextDamageTime = Time.time + _damageInterval;
            _hitThisAttack = false;
            if (_useLunge && _body != null)
            {
                _body.linearVelocity = Vector2.zero;
                _body.MovePosition(_body.position);
            }
        }

        private bool OpeningCanReach(Bounds body, float gap)
        {
            if (gap <= body.extents.x + _punchReach) return true;
            // Use the same closest-point distance as the authored dash travel,
            // with a small margin instead of attacking at its absolute limit.
            float requiredTravel = Mathf.Max(0f, gap - body.extents.x - .15f);
            return requiredTravel <= Mathf.Max(0f, _lungeDistance - .35f);
        }

        private bool OnScreen(Vector3 position)
        {
            if (_camera == null) _camera = Camera.main;
            if (_camera == null) return false;
            Vector3 point = _camera.WorldToViewportPoint(position);
            return point.z > 0f && point.x > 0.03f && point.x < 0.97f && point.y > 0f && point.y < 1f;
        }

        private float SafeLungeStep(float distance)
        {
            Bounds b = _collider.bounds;
            Vector2 direction = Vector2.right * AttackFacing;
            Vector2 size = new Vector2(Mathf.Max(0.05f, b.size.x - 0.04f),
                Mathf.Max(0.05f, b.size.y - 0.08f));
            int count = Physics2D.BoxCast(b.center, size, 0f, direction, _solids, _casts, distance + 0.04f);
            for (int i = 0; i < count; i++)
                if (_casts[i].collider != null && _casts[i].collider.attachedRigidbody != _body &&
                    Vector2.Dot(_casts[i].normal, direction) < -0.5f)
                    distance = Mathf.Min(distance, Mathf.Max(0f, _casts[i].distance - 0.04f));
            // Sample the entire leading-foot path, including the current edge.
            // Kinematic tomatoes must not dash across a pit and hover over it.
            for (float sample = 0f; sample <= distance + 0.099f; sample += 0.1f)
            {
                float offset = Mathf.Min(sample, distance);
                Vector2 foot = new Vector2(b.center.x + AttackFacing * (b.extents.x + offset), b.min.y + 0.1f);
                if (Physics2D.Raycast(foot, Vector2.down, _solids, _casts, 0.3f) == 0)
                    return Mathf.Max(0f, offset - 0.1f);
            }
            return distance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void TryDamage(Collider2D other)
        {
            // Trigger callbacks can reach disabled behaviours. Keep the hurtbox
            // enabled, but suppress outgoing damage while stunned or inactive.
            if (!isActiveAndEnabled || _useAttackAnimation || (_hitStun != null && _hitStun.IsStunned))
            {
                return;
            }

            // Contact is reported every physics step, so the interval is what keeps a
            // single touch from draining the whole health bar at once.
            if (Time.time < _nextDamageTime)
            {
                return;
            }

            // A player's colliders may sit on child objects, but they all report the
            // same attached body, so the health is looked up from that body instead.
            // Anything without PlayerHealth is left untouched.
            Rigidbody2D touchingBody = other.attachedRigidbody;
            if (touchingBody == null || !touchingBody.TryGetComponent(out PlayerHealth health))
            {
                return;
            }

            _nextDamageTime = Time.time + _damageInterval;
            health.TakeDamage(_damage);
        }
    }
}

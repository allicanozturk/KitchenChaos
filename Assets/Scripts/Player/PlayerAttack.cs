using System;
using System.Collections.Generic;
using KitchenChaos.Enemy;
using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Owns facing and the melee clock. Presentation reads the same phases that
    /// gate damage; no Animator event or delayed coroutine can deal another hit.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    [DefaultExecutionOrder(-40)]
    public sealed class PlayerAttack : MonoBehaviour
    {
        public enum AttackPhase { Idle, Windup, Active, Recovery }

        [SerializeField] private Transform _attackOrigin;
        [SerializeField, Min(0f)] private float _attackRadius = 0.75f;
        [SerializeField, Min(1)] private int _attackDamage = 1;
        [SerializeField, Min(0f)] private float _attackCooldown = 0.4f;
        [SerializeField] private LayerMask _targetLayers;
        [SerializeField, Min(0f)] private float _windupDuration = 0.1f;
        [SerializeField, Min(0.02f)] private float _activeDuration = 0.12f;
        [SerializeField, Min(0f)] private float _recoveryDuration = 0.18f;
        [SerializeField] private bool _allowCrouchedAttack;
        [SerializeField] private Vector2 _crouchOriginLocalPosition = new(1.2f, -0.55f);
        public bool IsCrouchedSwing { get; private set; }

        public AttackPhase Phase { get; private set; }
        public bool IsAttacking => Phase != AttackPhase.Idle;
        public int FacingDirection { get; private set; } = 1;
        public float PhaseProgress => !IsAttacking ? 0f :
            Mathf.Clamp01((Time.time - _phaseStartedAt) / Mathf.Max(0.0001f, _phaseDuration));

        /// <summary>
        /// Raised once per accepted swing, whether or not it connects, so presentation
        /// can react without the attack rules having to know about it.
        /// </summary>
        public event Action Attacked;
        public event Action ActiveStarted;
        public event Action<Vector2> HitConnected;

        // Reused across swings so a query that runs several times a second does not
        // allocate a fresh buffer every time.
        private readonly List<Collider2D> _overlapResults = new();
        private readonly HashSet<EnemyHealth> _damagedThisSwing = new();

        private PlayerInputReader _input;
        private PlayerMobility _mobility;
        private PlayerRangedAttack _ranged;
        private PlayerSpinAttack _spin;
        private ContactFilter2D _targetFilter;
        private bool _attackPressLatched;
        private float _nextAttackTime;
        private float _phaseStartedAt;
        private float _phaseDuration;
        private Vector3 _originRightLocalPosition;

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();
            _mobility = GetComponent<PlayerMobility>();
            _ranged = GetComponent<PlayerRangedAttack>();
            _spin = GetComponent<PlayerSpinAttack>();

            if (_attackOrigin == null)
            {
                // Fail once and loudly instead of querying the player's own pivot and
                // leaving the designer to wonder why the reach is wrong.
                Debug.LogError($"{nameof(PlayerAttack)} needs an Attack Origin transform assigned.", this);
                enabled = false;
                return;
            }

            Vector3 authoredOrigin = _attackOrigin.localPosition;
            _originRightLocalPosition = new Vector3(Mathf.Abs(authoredOrigin.x), authoredOrigin.y, authoredOrigin.z);
            FacingDirection = authoredOrigin.x < 0f ? -1 : 1;
            ApplyFacingToOrigin();

            // Enemy hurtboxes are trigger colliders, so the query has to include them
            // rather than inherit the project-wide "queries hit triggers" setting.
            _targetFilter.useTriggers = true;
            _targetFilter.SetLayerMask(_targetLayers);
        }

        private void Update()
        {
            if (_mobility != null && _mobility.IsParried)
            { _attackPressLatched = false; return; }
            if (_input.IsGameplayBlocked)
                return;

            // Facing is gameplay state, not a side effect of a sprite flip. Lock it
            // for the whole swing so one attack cannot sweep both sides by turning.
            if (_mobility != null && _mobility.isActiveAndEnabled && _mobility.IsDashing)
                FacingDirection = _mobility.DashDirection;
            else if (_spin != null && _spin.IsSpinning)
                FacingDirection = _spin.Facing;
            else if (_ranged != null && _ranged.IsThrowing)
                FacingDirection = _ranged.ThrowFacing;
            else if (!IsAttacking && _ranged != null && _ranged.isActiveAndEnabled &&
                _input.RangedHeld && Mathf.Abs(_input.Aim.x) >= 0.35f)
                FacingDirection = _input.Aim.x < 0f ? -1 : 1;
            else if (!IsAttacking && Mathf.Abs(_input.Horizontal) >= 0.01f)
                FacingDirection = _input.Horizontal > 0f ? 1 : -1;
            ApplyFacingToOrigin();

            // Latching in Update guarantees a press landing between two physics steps
            // is still seen by FixedUpdate, where the overlap query belongs.
            if (_input.AttackPressedThisFrame)
            {
                _attackPressLatched = true;
            }
        }

        private void FixedUpdate()
        {
            bool pressedThisStep = _attackPressLatched;
            _attackPressLatched = false;

            if ((_ranged != null && _ranged.IsThrowing) || (_spin != null && _spin.IsSpinning)) return;

            if (_input.IsGameplayBlocked || (_mobility != null && _mobility.isActiveAndEnabled &&
                (_mobility.IsDashing || _mobility.IsHurt || (_mobility.IsCrouching && !_allowCrouchedAttack))))
            {
                if (IsAttacking)
                    ResetTransientState();
                return;
            }

            // A press during any busy phase is discarded, including the step that
            // finishes recovery. Holding a button never queues another attack.
            if (IsAttacking)
            {
                AdvanceSwing();
                return;
            }

            // A press during the cooldown is dropped rather than queued, so holding
            // the button cannot bank swings that all land the moment it expires.
            if (!pressedThisStep || Time.time < _nextAttackTime)
            {
                return;
            }

            _nextAttackTime = Time.time + _attackCooldown;
            IsCrouchedSwing = _allowCrouchedAttack && _mobility != null && _mobility.IsCrouching;
            _damagedThisSwing.Clear();
            SetPhase(AttackPhase.Windup, Mathf.Max(0f, _windupDuration));
            ApplyFacingToOrigin();
            Attacked?.Invoke();
            if (IsAttacking && !_input.IsGameplayBlocked)
                AdvanceSwing();
        }

        public void ResetTransientState()
        {
            _attackPressLatched = false;
            _nextAttackTime = 0f;
            Phase = AttackPhase.Idle;
            IsCrouchedSwing = false;
            _phaseStartedAt = 0f;
            _phaseDuration = 0f;
            _overlapResults.Clear();
            _damagedThisSwing.Clear();
        }

        private void SetPhase(AttackPhase phase, float duration)
        {
            Phase = phase;
            _phaseStartedAt = Time.time;
            _phaseDuration = duration;
        }

        private void AdvanceSwing()
        {
            bool finished = Time.time - _phaseStartedAt + 0.00001f >= _phaseDuration;
            if (Phase == AttackPhase.Windup && finished)
            {
                // At least one physics sample, even if Inspector timings are tiny.
                SetPhase(AttackPhase.Active, Mathf.Max(Time.fixedDeltaTime, _activeDuration));
                ActiveStarted?.Invoke();
            }
            else if (Phase == AttackPhase.Active && finished)
            {
                SetPhase(AttackPhase.Recovery, Mathf.Max(0f, _recoveryDuration));
            }
            else if (Phase == AttackPhase.Recovery && finished)
            {
                Phase = AttackPhase.Idle;
            }

            if (Phase == AttackPhase.Active && !_input.IsGameplayBlocked)
                QueryActiveHits();
        }

        private void QueryActiveHits()
        {
            int hitCount = Physics2D.OverlapCircle(_attackOrigin.position, _attackRadius, _targetFilter, _overlapResults);
            for (int i = 0; i < hitCount; i++)
            {
                // A damage listener may cancel the swing (death/disable/respawn).
                if (Phase != AttackPhase.Active || _input.IsGameplayBlocked)
                    break;

                // One enemy can answer the query with several colliders, and its
                // colliders may sit on child objects, so health is resolved upwards
                // and each enemy is damaged at most once per swing.
                EnemyHealth target = _overlapResults[i].GetComponentInParent<EnemyHealth>();
                if (target == null || !target.isActiveAndEnabled || target.CurrentHealth <= 0 ||
                    !_damagedThisSwing.Add(target))
                {
                    continue;
                }

                // Cache the point before lethal damage disables the enemy. The
                // impact is rendered by the player and survives the enemy's death.
                Vector2 hitPoint = _overlapResults[i].ClosestPoint(_attackOrigin.position);
                var shield = target.GetComponent<BroccoliShieldEnemy>();
                // Only ordinary spatula swings provoke a parry. Returning is essential:
                // ReceiveShieldParry cancels this swing and clears the overlap buffer.
                if (shield != null && shield.TryParryMelee(_mobility, transform.position)) return;
                int before = target.CurrentHealth;
                target.TakeDamage(_attackDamage, true, transform.position);
                if (target != null && target.CurrentHealth < before &&
                    Phase == AttackPhase.Active && !_input.IsGameplayBlocked)
                    HitConnected?.Invoke(hitPoint);
            }
        }

        private void ApplyFacingToOrigin()
        {
            if (_attackOrigin == null)
                return;

            Vector3 position = _originRightLocalPosition;
            if (IsAttacking && IsCrouchedSwing)
                position = new Vector3(_crouchOriginLocalPosition.x, _crouchOriginLocalPosition.y, position.z);
            position.x *= FacingDirection;
            _attackOrigin.localPosition = position;
        }

        private void OnDisable()
        {
            ResetTransientState();
        }

        private void OnDrawGizmosSelected()
        {
            if (_attackOrigin == null)
            {
                return;
            }

            // The reach is invisible in the Scene view otherwise, which makes placing
            // the origin and tuning the radius guesswork.
            Gizmos.color = Phase == AttackPhase.Active ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(_attackOrigin.position, _attackRadius);
        }
    }
}

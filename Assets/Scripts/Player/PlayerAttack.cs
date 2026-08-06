using System.Collections.Generic;
using KitchenChaos.Enemy;
using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Performs the player's close-range attack as a cooldown-gated overlap query
    /// around an authored origin, damaging every enemy it finds.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Transform _attackOrigin;
        [SerializeField, Min(0f)] private float _attackRadius = 0.75f;
        [SerializeField, Min(1)] private int _attackDamage = 1;
        [SerializeField, Min(0f)] private float _attackCooldown = 0.4f;
        [SerializeField] private LayerMask _targetLayers;

        // Reused across swings so a query that runs several times a second does not
        // allocate a fresh buffer every time.
        private readonly List<Collider2D> _overlapResults = new();
        private readonly HashSet<EnemyHealth> _damagedThisSwing = new();

        private PlayerInputReader _input;
        private ContactFilter2D _targetFilter;
        private bool _attackPressLatched;
        private float _nextAttackTime;

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();

            if (_attackOrigin == null)
            {
                // Fail once and loudly instead of querying the player's own pivot and
                // leaving the designer to wonder why the reach is wrong.
                Debug.LogError($"{nameof(PlayerAttack)} needs an Attack Origin transform assigned.", this);
                enabled = false;
                return;
            }

            // Enemy hurtboxes are trigger colliders, so the query has to include them
            // rather than inherit the project-wide "queries hit triggers" setting.
            _targetFilter.useTriggers = true;
            _targetFilter.SetLayerMask(_targetLayers);
        }

        private void Update()
        {
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

            // A press during the cooldown is dropped rather than queued, so holding
            // the button cannot bank swings that all land the moment it expires.
            if (!pressedThisStep || Time.time < _nextAttackTime)
            {
                return;
            }

            _nextAttackTime = Time.time + _attackCooldown;
            Attack();
        }

        private void Attack()
        {
            _damagedThisSwing.Clear();

            int hitCount = Physics2D.OverlapCircle(_attackOrigin.position, _attackRadius, _targetFilter, _overlapResults);
            for (int i = 0; i < hitCount; i++)
            {
                // One enemy can answer the query with several colliders, and its
                // colliders may sit on child objects, so health is resolved upwards
                // and each enemy is damaged at most once per swing.
                EnemyHealth target = _overlapResults[i].GetComponentInParent<EnemyHealth>();
                if (target == null || !_damagedThisSwing.Add(target))
                {
                    continue;
                }

                target.TakeDamage(_attackDamage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_attackOrigin == null)
            {
                return;
            }

            // The reach is invisible in the Scene view otherwise, which makes placing
            // the origin and tuning the radius guesswork.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_attackOrigin.position, _attackRadius);
        }
    }
}

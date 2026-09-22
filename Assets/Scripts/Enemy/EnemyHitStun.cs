using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Optional gameplay reaction to a successful hit. Patrol and contact damage
    /// consult the same state; the hurtbox remains available for follow-up hits.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyHitStun : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _duration = 0.45f;
        [SerializeField, Min(0f)] private float _resistanceAfterStun = 0f;

        // Only accepted stuns interrupt attacks; health damage is independent.
        public event System.Action StunStarted;

        public bool IsStunned => isActiveAndEnabled && Time.time < _stunnedUntil;

        private EnemyHealth _health;
        private Rigidbody2D _rigidbody;
        private float _stunnedUntil;
        private float _resistantUntil;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _stunnedUntil = 0f;
            _resistantUntil = 0f;
            _health.Damaged += OnDamaged;
            _health.Restored += ClearStun;
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.Damaged -= OnDamaged;
                _health.Restored -= ClearStun;
            }

            _stunnedUntil = 0f;
            _resistantUntil = 0f;
        }

        private void OnDamaged()
        {
            if (!_health.LastHitCanStun || _duration <= 0f || Time.time < _resistantUntil)
                return;

            // Refresh the window instead of banking extra seconds per hit.
            _stunnedUntil = Time.time + _duration;
            // Opt-in enemies cannot have this stun extended by repeated hits.
            // Zero preserves the original refresh-on-hit behaviour for tomatoes.
            if (_resistanceAfterStun > 0f)
                _resistantUntil = _stunnedUntil + _resistanceAfterStun;

            if (_rigidbody != null && _rigidbody.bodyType == RigidbodyType2D.Kinematic)
            {
                // Patrol may already have queued a move earlier in this FixedUpdate.
                // The last MovePosition before simulation wins, so replace it now.
                _rigidbody.linearVelocity = Vector2.zero;
                _rigidbody.MovePosition(_rigidbody.position);
            }
            StunStarted?.Invoke();
        }

        private void ClearStun()
        {
            _stunnedUntil = 0f;
            _resistantUntil = 0f;
        }
    }
}

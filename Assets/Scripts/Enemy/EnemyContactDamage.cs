using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Damages the player while it stays in contact with this enemy.
    /// Kept separate from <see cref="EnemyPatrol"/> so a stationary hazard can reuse
    /// the same contact damage rules without inheriting any movement.
    /// </summary>
    public sealed class EnemyContactDamage : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _damage = 1;
        [SerializeField, Min(0f)] private float _damageInterval = 1f;

        private float _nextDamageTime;

        private void Awake()
        {
            // Without a trigger collider the enemy is silently harmless, so fail once
            // and loudly instead of leaving the designer to guess.
            if (!TryGetComponent(out Collider2D damageCollider) || !damageCollider.isTrigger)
            {
                Debug.LogError($"{nameof(EnemyContactDamage)} needs a Collider2D with Is Trigger enabled.", this);
                enabled = false;
            }
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

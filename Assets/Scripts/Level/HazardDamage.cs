using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Level
{
    /// <summary>
    /// Damages the player while it stays inside this hazard's trigger.
    /// Spikes and lava are the same component with different Inspector values, so a new
    /// hazard needs no new script, and the damage rules match <see cref="Enemy.EnemyContactDamage"/>
    /// to keep contact damage predictable wherever the player meets it.
    /// </summary>
    public sealed class HazardDamage : MonoBehaviour
    {
        // A zero interval would let contact damage land on every physics step, which is
        // exactly what the interval exists to prevent, so the Inspector cannot reach it.
        private const float MinDamageInterval = 0.05f;

        [SerializeField, Min(1)] private int _damage = 1;
        [SerializeField, Min(MinDamageInterval)] private float _damageInterval = 1f;
        [SerializeField] private bool _instantKill;

        private float _nextDamageTime;

        private void Awake()
        {
            // Without a trigger collider the hazard is silently harmless, so fail once
            // and loudly instead of leaving the designer to guess.
            if (!TryGetComponent(out Collider2D hazardCollider) || !hazardCollider.isTrigger)
            {
                Debug.LogError($"{nameof(HazardDamage)} needs a Collider2D with Is Trigger enabled.", this);
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

            // An instant kill is still routed through TakeDamage so death, respawn and
            // the health restore stay owned by PlayerHealth; only the amount changes.
            // MaxHealth empties the bar from any starting value without this component
            // having to read the player's current state.
            health.TakeDamage(_instantKill ? health.MaxHealth : _damage);
        }
    }
}

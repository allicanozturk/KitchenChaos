using System;
using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Owns an enemy's hit points and removes it from the scene when they run out.
    /// Kept separate from <see cref="EnemyPatrol"/> and <see cref="EnemyContactDamage"/>
    /// so a stationary or harmless enemy can still be killed by the same attack.
    /// </summary>
    public sealed class EnemyHealth : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _maxHealth = 3;

        public int MaxHealth => _maxHealth;

        public int CurrentHealth { get; private set; }
        public bool LastHitCanStun { get; private set; } = true;

        /// <summary>
        /// Raised synchronously for accepted damage, before death is processed, so
        /// hit reactions can suppress contact damage in the same physics step.
        /// </summary>
        public event Action Damaged;

        /// <summary>
        /// Raised once, while the enemy is still alive in the scene, so presentation can
        /// react before it is deactivated and destroyed.
        /// </summary>
        public event Action Died;
        public event Action Restored;

        private bool _isDead;

        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }

        public void TakeDamage(int amount) => TakeDamage(amount, true);

        /// <summary>Directional player hits may be intercepted by an optional shield.</summary>
        public void TakeDamage(int amount, bool canStun, Vector2 sourcePosition)
        {
            if (amount <= 0 || _isDead) return;
            var shield = GetComponent<BroccoliShieldEnemy>();
            if (shield != null && shield.TryBlock(sourcePosition)) return;
            TakeDamage(amount, canStun);
        }

        /// <summary>Retry heals survivors; it never revives destroyed enemies or emits a hit.</summary>
        public void RestoreForPlayerRespawn()
        {
            if (_isDead || CurrentHealth <= 0 || !isActiveAndEnabled) return;
            CurrentHealth = _maxHealth;
            LastHitCanStun = true;
            Restored?.Invoke();
        }

        public void TakeDamage(int amount, bool canStun)
        {
            // Ignoring non-positive damage keeps a miscalculating caller from healing
            // the enemy through the damage path.
            if (amount <= 0)
            {
                return;
            }

            // Destroy only takes effect at the end of the frame, so damage that lands
            // after death must not run the death path a second time.
            if (_isDead)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            LastHitCanStun = canStun;
            Damaged?.Invoke();
            LogHealth();

            if (CurrentHealth == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;

            // Announced first: deactivating below also disables every listener on this
            // enemy, so anything that wants to react has to hear about it before that.
            Died?.Invoke();

            // Deactivating is what actually stops the enemy this frame: it ends the
            // patrol step and the contact damage triggers immediately, instead of
            // leaving them running until Destroy is processed.
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void LogHealth()
        {
#if UNITY_EDITOR
            // Enemy health has no UI yet, so play tests need a way to read it without
            // shipping log noise in the player build.
            Debug.Log($"Enemy Health: {CurrentHealth}/{_maxHealth}", this);
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Take Damage (1)")]
        private void DebugTakeDamage()
        {
            TakeDamage(1);
        }
#endif
    }
}

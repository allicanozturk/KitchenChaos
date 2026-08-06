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

        private bool _isDead;

        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }

        public void TakeDamage(int amount)
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
            LogHealth();

            if (CurrentHealth == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;

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

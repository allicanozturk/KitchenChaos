using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Owns the player's hit points and decides what happens when they run out.
    /// </summary>
    [RequireComponent(typeof(PlayerRespawn))]
    public sealed class PlayerHealth : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _maxHealth = 3;

        public int MaxHealth => _maxHealth;

        public int CurrentHealth { get; private set; }

        private PlayerRespawn _respawn;
        private int _lastDeathFrame = -1;

        private void Awake()
        {
            _respawn = GetComponent<PlayerRespawn>();
            CurrentHealth = _maxHealth;
        }

        public void TakeDamage(int amount)
        {
            // Ignoring non-positive damage keeps a miscalculating caller from healing
            // the player through the damage path.
            if (amount <= 0)
            {
                return;
            }

            // Damage that arrives in the same frame as a death belongs to the life
            // that just ended, so it must not chip away at the restored health or
            // send the player through a second respawn.
            if (Time.frameCount == _lastDeathFrame)
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
            // Stamped before respawning so any further damage this frame is already
            // rejected by the time health is back at full.
            _lastDeathFrame = Time.frameCount;

            // Death carries no state of its own yet, but it stays a separate step so
            // future feedback can hook in without changing the damage rules.
            Respawn();
        }

        private void Respawn()
        {
            _respawn.Respawn();

            CurrentHealth = _maxHealth;
            LogHealth();
        }

        private void LogHealth()
        {
#if UNITY_EDITOR
            // Health has no UI yet, so play tests need a way to read it without
            // shipping log noise in the player build.
            Debug.Log($"Health: {CurrentHealth}/{_maxHealth}", this);
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

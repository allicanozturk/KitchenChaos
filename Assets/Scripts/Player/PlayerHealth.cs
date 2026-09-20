using System;
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
        [SerializeField, Min(0f)] private float _hurtInvulnerability = 0.75f;
        [SerializeField, Min(0f)] private float _respawnInvulnerability = 0.8f;
        [SerializeField, Min(0f)] private float _deathDelay = 0.35f;

        public int MaxHealth => _maxHealth;

        public int CurrentHealth { get; private set; }

        public bool IsDead { get; private set; }

        public bool IsInvulnerable => !IsDead &&
            (Time.time < _invulnerableUntil || Time.frameCount == _lastDamageFrame ||
             Time.frameCount == _lastTransitionFrame);

        public float DeathProgress => !IsDead ? 0f :
            (_deathDelay <= 0f ? 1f : Mathf.Clamp01(1f - _deathRemaining / _deathDelay));

        /// <summary>
        /// Raised once per damage tick that actually landed, so presentation stays in
        /// step with the rules that reject a hit instead of guessing at them.
        /// </summary>
        public event Action Damaged;
        public event Action Died;

        private PlayerRespawn _respawn;
        private PlayerMobility _mobility;
        private float _invulnerableUntil;
        private float _deathRemaining;
        private int _lastDamageFrame = -1;
        private int _lastTransitionFrame = -1;

        private void Awake()
        {
            _respawn = GetComponent<PlayerRespawn>();
            _mobility = GetComponent<PlayerMobility>();
            CurrentHealth = _maxHealth;
        }

        private void OnEnable()
        {
            _respawn.Respawning += OnRespawning;
        }

        private void OnDisable()
        {
            if (_respawn != null)
                _respawn.Respawning -= OnRespawning;

            // Cancelling this component must not leave input/physics suspended.
            if (IsDead)
            {
                OnRespawning();
                _respawn?.ResumeControls();
            }
        }

        private void Update()
        {
            if (!IsDead)
                return;

            _deathRemaining = Mathf.Max(0f, _deathRemaining - Time.deltaTime);
            if (_deathRemaining == 0f)
                _respawn.Respawn();
        }

        public void TakeDamage(int amount) => TakeDamage(amount, true);

        public void TakeDamage(int amount, bool allowDashProtection)
        {
            if (amount <= 0 || !CanReceiveDamage() || IsInvulnerable)
                return;
            if (allowDashProtection && _mobility != null && _mobility.IsDodging)
                return;

            ApplyDamage(amount);
        }

        /// <summary>
        /// Lethal pits bypass ordinary protection, but not death or stale callbacks
        /// from the frame in which this player was teleported to a new life.
        /// </summary>
        public void Kill()
        {
            if (CanReceiveDamage())
                ApplyDamage(CurrentHealth);
        }

        private bool CanReceiveDamage()
        {
            return isActiveAndEnabled && !IsDead && Time.frameCount != _lastTransitionFrame;
        }

        private void ApplyDamage(int amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            _lastDamageFrame = Time.frameCount;

            // Establish all gates BEFORE callbacks, including audio/visual events.
            if (CurrentHealth == 0)
            {
                IsDead = true;
                _lastTransitionFrame = Time.frameCount;
                _deathRemaining = _deathDelay;
                _respawn.SuspendForDeath();
            }
            else
            {
                _invulnerableUntil = Time.time + _hurtInvulnerability;
            }

            LogHealth();
            Damaged?.Invoke();
            if (!IsDead)
                return;

            Died?.Invoke();
            if (_deathRemaining <= 0f && IsDead)
                _respawn.Respawn();
        }

        private void OnRespawning()
        {
            // Runs while controls/physics are still suspended, before teleport.
            _lastTransitionFrame = Time.frameCount;
            CurrentHealth = _maxHealth;
            IsDead = false;
            _deathRemaining = 0f;
            _invulnerableUntil = Time.time + _respawnInvulnerability;
            LogHealth();
        }

        private void LogHealth()
        {
#if UNITY_EDITOR
            // Editor-only diagnostics; the HUD reads CurrentHealth directly.
            Debug.Log($"Health: {CurrentHealth}/{_maxHealth}", this);
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Take Damage (1)")]
        private void DebugTakeDamage()
        {
            TakeDamage(1);
        }

        [ContextMenu("Force Death (bypass protection)")]
        private void DebugKill()
        {
            if (Application.isPlaying)
                Kill();
        }
#endif
    }
}

using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Shows accepted damage, invulnerability and death through sprite color only.
    /// The player's physics transform and Animator remain untouched.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerHealth))]
    public sealed class PlayerDamageFeedback : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _hitColor = new(1f, 0.3f, 0.2f, 1f);
        [SerializeField, Min(0.01f)] private float _hitDuration = 0.12f;
        [SerializeField, Range(0f, 0.95f)] private float _deathFadeStart;
        [SerializeField, Range(0f, 1f)] private float _deathTintStrength = 0.85f;

        private const float PulseFrequency = 4f;
        private const float MinimumPulseAlpha = 0.3f;
        private const float MaximumPulseAlpha = 0.85f;

        private PlayerHealth _health;
        private PlayerRespawn _respawn;
        private PlayerMobility _mobility;
        private Color _restColor;
        private float _hitRemaining;
        private bool _initialized;

        private void Awake()
        {
            _health = GetComponent<PlayerHealth>();
            _respawn = GetComponent<PlayerRespawn>();
            _mobility = GetComponent<PlayerMobility>();

            if (_spriteRenderer == null || _health == null || _respawn == null)
            {
                Debug.LogError(
                    $"{nameof(PlayerDamageFeedback)} needs a SpriteRenderer, PlayerHealth and PlayerRespawn.", this);
                enabled = false;
                return;
            }

            _restColor = _spriteRenderer.color;
            _initialized = true;
        }

        private void OnEnable()
        {
            if (!_initialized)
                return;

            _health.Damaged += OnDamaged;
            _respawn.Respawned += OnRespawned;
            OnRespawned();
        }

        private void LateUpdate()
        {
            Color color = _restColor;
            if (_health.IsDead)
            {
                color = Color.Lerp(_restColor, _hitColor, _deathTintStrength);
                // The optional death pack needs an opaque pose/hold before fading.
                // Default zero retains the earlier linear fade on other prefabs.
                float fade = Mathf.InverseLerp(Mathf.Clamp(_deathFadeStart, 0f, 0.95f), 1f,
                    _health.DeathProgress);
                color.a = _restColor.a * (1f - fade);
            }
            else if (_hitRemaining > 0f)
            {
                float progress = Mathf.Clamp01(1f - _hitRemaining / Mathf.Max(0.01f, _hitDuration));
                color = Color.Lerp(_hitColor, _restColor, progress);
                color.a = _restColor.a;
            }
            else if (_mobility != null && _mobility.IsHurtVisual)
            {
                // Show the authored reaction before pulsing; this changes only
                // opacity, never the actual health/invulnerability timer.
                color.a = _restColor.a;
            }
            else if (_health.IsInvulnerable)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * Mathf.PI * 2f * PulseFrequency);
                color.a = _restColor.a * Mathf.Lerp(MinimumPulseAlpha, MaximumPulseAlpha, pulse);
            }

            _spriteRenderer.color = color;
            _hitRemaining = Mathf.Max(0f, _hitRemaining - Time.deltaTime);
        }

        private void OnDamaged()
        {
            _hitRemaining = Mathf.Max(0.01f, _hitDuration);
        }

        private void OnRespawned()
        {
            _hitRemaining = 0f;
            RestoreColor();
        }

        private void RestoreColor()
        {
            if (_initialized && _spriteRenderer != null)
                _spriteRenderer.color = _restColor;
        }

        private void OnDisable()
        {
            if (_initialized)
            {
                if (_health != null)
                    _health.Damaged -= OnDamaged;
                if (_respawn != null)
                    _respawn.Respawned -= OnRespawned;
            }

            _hitRemaining = 0f;
            RestoreColor();
        }
    }
}

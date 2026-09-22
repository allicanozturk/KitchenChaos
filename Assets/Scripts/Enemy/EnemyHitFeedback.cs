using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Presents health loss without changing damage, patrol, or physics rules.
    /// Animate a visual child, never the Rigidbody/collider root.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyHitFeedback : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _healthFill;
        [SerializeField] private Color _hitColor = new(1f, 0.3f, 0.25f, 1f);
        [SerializeField, Min(0.01f)] private float _hitDuration = 0.18f;
        [SerializeField, Min(0f)] private float _shakeDistance = 0.07f;
        [SerializeField, Range(0f, 0.3f)] private float _squashAmount = 0.12f;

        private EnemyHealth _health;
        private Transform _visual;
        private Vector3 _restPosition;
        private Vector3 _restScale;
        private Vector3 _fullBarScale;
        private Color _restColor;
        private int _shownHealth;
        private float _remaining;
        private bool _initialized;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            if (_spriteRenderer == null || _spriteRenderer.transform == transform ||
                !_spriteRenderer.transform.IsChildOf(transform))
            {
                Debug.LogError($"{nameof(EnemyHitFeedback)} needs a SpriteRenderer on a visual child.", this);
                enabled = false;
                return;
            }

            _visual = _spriteRenderer.transform;
            _restPosition = _visual.localPosition;
            _restScale = _visual.localScale;
            _restColor = _spriteRenderer.color;
            _fullBarScale = _healthFill != null ? _healthFill.localScale : Vector3.one;
            _initialized = true;
        }

        private void OnEnable()
        {
            if (!_initialized)
                return;

            // EnemyHealth.Awake may run later on first activation. Its serialized
            // maximum is already available, unlike CurrentHealth at that moment.
            _shownHealth = _health.CurrentHealth > 0 ? _health.CurrentHealth : _health.MaxHealth;
            _health.Restored += OnRestored;
            _remaining = 0f;
            UpdateBar(_shownHealth);
            RestoreVisual();
        }

        private void LateUpdate()
        {
            int current = _health.CurrentHealth;
            if (current != _shownHealth)
            {
                if (current < _shownHealth)
                    _remaining = _hitDuration;

                _shownHealth = current;
                UpdateBar(current);
            }

            if (_remaining <= 0f)
                return;

            float duration = Mathf.Max(0.01f, _hitDuration);
            float progress = Mathf.Clamp01(1f - _remaining / duration);
            float strength = 1f - progress;
            _spriteRenderer.color = Color.Lerp(_hitColor, _restColor, progress);
            _visual.localPosition = _restPosition + Vector3.right *
                (Mathf.Sin(progress * Mathf.PI * 4f) * _shakeDistance * strength);
            _visual.localScale = Vector3.Scale(_restScale,
                new Vector3(1f + _squashAmount * strength, 1f - _squashAmount * strength, 1f));

            _remaining = Mathf.Max(0f, _remaining - Time.deltaTime);
            if (_remaining == 0f)
                RestoreVisual();
        }

        private void UpdateBar(int current)
        {
            if (_healthFill == null)
                return;

            float fraction = Mathf.Clamp01((float)current / Mathf.Max(1, _health.MaxHealth));
            _healthFill.localScale = new Vector3(_fullBarScale.x * fraction, _fullBarScale.y, _fullBarScale.z);
        }

        private void OnRestored()
        {
            _remaining = 0f;
            _shownHealth = _health.CurrentHealth;
            UpdateBar(_shownHealth);
            RestoreVisual();
        }

        private void RestoreVisual()
        {
            if (!_initialized || _visual == null || _spriteRenderer == null)
                return;

            _visual.localPosition = _restPosition;
            _visual.localScale = _restScale;
            _spriteRenderer.color = _restColor;
        }

        private void OnDisable()
        {
            if (_health != null) _health.Restored -= OnRestored;
            _remaining = 0f;
            RestoreVisual();
        }
    }
}

using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>Sprite-only presentation driven by the existing ranged attack clock.</summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(40)]
    public sealed class PeaRangedVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private SpriteRenderer _chargeIndicator;
        // Idle, half bend, full cheeks, spit, rise back up.
        [SerializeField] private Sprite[] _frames = new Sprite[5];
        [SerializeField] private Vector2[] _mouthPoints = new Vector2[5];
        [SerializeField] private Sprite _hurtUpright;
        [SerializeField] private Sprite _hurtBent;
        [SerializeField] private Vector2 _hurtUprightMouth;
        [SerializeField] private Vector2 _hurtBentMouth;
        [SerializeField, Min(0.05f)] private float _hurtDuration = 0.18f;
        private EnemyHealth _health;
        private float _hurtUntil;
        private bool _showingHurt;
        private bool _hurtIsBent;
        private int _direction = -1;
        private int _frame;
        private float _recoveryTime;
        private bool _recovering;
        private bool _afterShot;
        private bool _initialized;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            bool valid = _spriteRenderer != null && _muzzle != null &&
                _chargeIndicator != null && _frames != null && _frames.Length == 5 &&
                _mouthPoints != null && _mouthPoints.Length == 5;
            if (valid)
                for (int i = 0; i < 5; i++) valid &= _frames[i] != null;
            if (!valid)
            {
                Debug.LogError($"{nameof(PeaRangedVisual)} needs five poses and mouth anchors.", this);
                enabled = false;
                return;
            }
            _initialized = true;
            ApplyFrame(0);
        }

        private void OnEnable()
        {
            if (_health != null) _health.Damaged += OnDamaged;
            if (_health != null) _health.Restored += OnRestored;
        }

        private void OnRestored()
        {
            _hurtUntil = 0f;
            _recovering = false;
            _recoveryTime = 0f;
            if (_initialized) ApplyFrame(0);
        }

        private void OnDamaged()
        {
            // Purely visual, including during stun resistance. Never touch the
            // attack clock, health, facing lock or existing hit-shake transform.
            _hurtUntil = Time.time + _hurtDuration;
            if (_initialized) ApplyFrame(_frame);
        }

        public void ShowWindup(float progress, int direction)
        {
            if (!_initialized) return;
            _direction = direction;
            _recovering = false;
            ApplyFrame(progress < 0.2f ? 0 : progress < 0.58f ? 1 : 2);
        }

        public void ShowShot(int direction)
        {
            if (!_initialized) return;
            _direction = direction;
            // Called synchronously BEFORE the seed is instantiated, not in LateUpdate.
            // The firing pose and its mouth anchor take precedence over a grimace.
            _hurtUntil = 0f;
            ApplyFrame(3);
        }

        public void BeginRecovery(bool fired)
        {
            if (_frame == 0 && !fired) { _recovering = false; return; }
            _recovering = true;
            _afterShot = fired;
            _recoveryTime = 0f;
        }

        private void LateUpdate()
        {
            if (!_initialized || Time.deltaTime <= 0f) return;
            if (_recovering)
            {
                _recoveryTime += Time.deltaTime;
                ApplyFrame(_afterShot && _recoveryTime < 0.12f ? 3 : _recoveryTime < 0.3f ? 4 : 0);
                if (_recoveryTime >= 0.3f) _recovering = false;
            }
            // Also restore the underlying pose when a brief hurt overlay expires.
            ApplyFrame(_frame);
        }

        private void ApplyFrame(int frame)
        {
            _frame = frame;
            _hurtIsBent = frame == 1 || frame == 2 || frame == 3;
            Sprite hurt = _hurtIsBent ? _hurtBent : _hurtUpright;
            _showingHurt = Time.time < _hurtUntil && hurt != null;
            _spriteRenderer.sprite = _showingHurt ? hurt : _frames[frame];
            _spriteRenderer.flipX = _direction < 0;
            UpdateMouth();
        }

        private void UpdateMouth()
        {
            Vector2 point = _showingHurt
                ? (_hurtIsBent ? _hurtBentMouth : _hurtUprightMouth)
                : _mouthPoints[_frame];
            if (_direction < 0) point.x = -point.x;
            _muzzle.position = _spriteRenderer.transform.TransformPoint(point);
            _chargeIndicator.transform.position = _muzzle.position;
        }

        private void OnDisable()
        {
            if (_health != null) _health.Damaged -= OnDamaged;
            if (_health != null) _health.Restored -= OnRestored;
            _hurtUntil = 0f;
            _recovering = false;
            if (_initialized) ApplyFrame(0);
        }
    }
}

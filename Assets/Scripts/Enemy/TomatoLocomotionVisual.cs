using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Optional tomato presentation. Reads patrol motion without changing physics;
    /// hit feedback keeps sole ownership of the visual transform and tint.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(EnemyPatrol))]
    public sealed class TomatoLocomotionVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite[] _walkFrames;
        [SerializeField, Min(1f)] private float _framesPerSecond = 8f;
        [SerializeField, Min(0.01f)] private float _referenceSpeed = 1.2f;
        [SerializeField] private bool _initialFacingLeft = true;

        [Header("Hit expression")]
        [SerializeField] private Sprite _hurtSprite;
        [SerializeField, Min(0.05f)] private float _hurtDuration = 0.28f;
        [SerializeField] private Sprite[] _attackFrames = new Sprite[3];
        [SerializeField] private Sprite _lungeWindupSprite;
        private EnemyContactDamage _contactDamage;

        private EnemyHealth _health;
        private float _hurtRemaining;

        private const float MinimumSpeed = 0.01f;
        private const float TeleportDistance = 2f;

        private Rigidbody2D _rigidbody;
        private EnemyPatrol _patrol;
        private EnemyHitStun _hitStun;
        private Vector2 _lastPhysicsPosition;
        private float _lastFixedTime;
        private float _horizontalSpeed;
        private float _frameClock;
        private bool _facingLeft;
        private bool _bindingsValid;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _patrol = GetComponent<EnemyPatrol>();
            _hitStun = GetComponent<EnemyHitStun>();
            _health = GetComponent<EnemyHealth>();
            _contactDamage = GetComponent<EnemyContactDamage>();
        }

        private void OnEnable()
        {
            bool valid = _spriteRenderer != null &&
                         _spriteRenderer.transform != transform &&
                         _spriteRenderer.transform.IsChildOf(transform) &&
                         _idleSprite != null &&
                         _walkFrames != null && _walkFrames.Length > 0;
            if (valid)
            {
                for (int i = 0; i < _walkFrames.Length; i++)
                    valid &= _walkFrames[i] != null;
            }

            _bindingsValid = valid;
            if (!valid)
            {
                Debug.LogError($"{nameof(TomatoLocomotionVisual)} needs its renderer, idle sprite and walk frames assigned.", this);
                enabled = false;
                return;
            }

            _hurtRemaining = 0f;
            if (_health != null)
            {
                _health.Damaged += OnDamaged;
                _health.Restored += OnRestored;
            }

            _lastPhysicsPosition = _rigidbody.position;
            _lastFixedTime = Time.fixedTime;
            _horizontalSpeed = 0f;
            _frameClock = 0f;
            _facingLeft = _initialFacingLeft;
            ShowIdle();
        }

        private void LateUpdate()
        {
            // Keep the displayed frame frozen while the game is paused.
            if (Time.deltaTime <= 0f || !_bindingsValid ||
                _walkFrames == null || _walkFrames.Length == 0)
                return;

            SampleCompletedPhysicsStep();

            if (_contactDamage != null && _contactDamage.IsAttacking &&
                _attackFrames != null && _attackFrames.Length == 3 &&
                _attackFrames[Mathf.Clamp(_contactDamage.AttackPose, 0, 2)] != null &&
                !(_hitStun != null && _hitStun.IsStunned))
            {
                _hurtRemaining = 0f;
                _frameClock = 0f;
                _facingLeft = _contactDamage.AttackFacing < 0;
                _spriteRenderer.sprite = _contactDamage.IsPreparingLunge && _lungeWindupSprite != null
                    ? _lungeWindupSprite : _attackFrames[_contactDamage.AttackPose];
                _spriteRenderer.flipX = _facingLeft;
                return;
            }

            // This presenter remains the sole sprite owner. Hit feedback still
            // owns shake/tint/scale, while hit stun owns gameplay interruption.
            if (_hurtRemaining > 0f && _hurtSprite != null)
            {
                _hurtRemaining = Mathf.Max(0f, _hurtRemaining - Time.deltaTime);
                _frameClock = 0f;
                _spriteRenderer.sprite = _hurtSprite;
                _spriteRenderer.flipX = _facingLeft;
                return;
            }

            bool stopped = !_rigidbody.simulated || !_patrol.isActiveAndEnabled ||
                           (_hitStun != null && _hitStun.IsStunned) ||
                           Mathf.Abs(_horizontalSpeed) < MinimumSpeed;
            if (stopped)
            {
                _frameClock = 0f;
                ShowIdle();
                return;
            }

            _facingLeft = _horizontalSpeed < 0f;
            float speedRatio = Mathf.Clamp(Mathf.Abs(_horizontalSpeed) /
                                           Mathf.Max(0.01f, _referenceSpeed), 0.25f, 2f);
            float nextClock = _frameClock + Time.deltaTime * _framesPerSecond * speedRatio;
            _frameClock = float.IsNaN(nextClock) || float.IsInfinity(nextClock)
                ? 0f : Mathf.Repeat(nextClock, _walkFrames.Length);
            // Float wrap can round to the inclusive upper endpoint (observed 6
            // for six frames). Never use the animation clock directly as an index.
            if (_frameClock >= _walkFrames.Length) _frameClock = 0f;
            int frame = Mathf.Clamp(Mathf.FloorToInt(_frameClock), 0, _walkFrames.Length - 1);
            _spriteRenderer.sprite = _walkFrames[frame];
            _spriteRenderer.flipX = _facingLeft;
        }

        private void OnDamaged()
        {
            // Damaged is raised before Died even for a killing blow.
            // Let death presentation exclusively own the lethal reaction.
            if (_health == null || _health.CurrentHealth <= 0 || _hurtSprite == null)
                return;

            _hurtRemaining = _hurtDuration;
            _spriteRenderer.sprite = _hurtSprite;
            _spriteRenderer.flipX = _facingLeft;
        }

        private void SampleCompletedPhysicsStep()
        {
            float fixedTime = Time.fixedTime;
            float elapsed = fixedTime - _lastFixedTime;
            if (elapsed <= 0f)
                return;

            // Hold this measurement between physics steps: render-only frames
            // must not alternate between idle and walk at high refresh rates.
            Vector2 position = _rigidbody.position;
            Vector2 displacement = position - _lastPhysicsPosition;
            _horizontalSpeed = displacement.sqrMagnitude > TeleportDistance * TeleportDistance
                ? 0f
                : displacement.x / elapsed;
            _lastPhysicsPosition = position;
            _lastFixedTime = fixedTime;
        }

        private void ShowIdle()
        {
            if (!_bindingsValid || _spriteRenderer == null)
                return;

            if (_idleSprite != null)
                _spriteRenderer.sprite = _idleSprite;
            _spriteRenderer.flipX = _facingLeft;
        }

        private void OnRestored()
        {
            _hurtRemaining = 0f;
            _frameClock = 0f;
            ShowIdle();
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.Damaged -= OnDamaged;
                _health.Restored -= OnRestored;
            }
            _hurtRemaining = 0f;
            _horizontalSpeed = 0f;
            _frameClock = 0f;
            _facingLeft = _initialFacingLeft;
            ShowIdle();
        }
    }
}

using KitchenChaos.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KitchenChaos.Enemy
{
    /// <summary>Stationary prototype: readable windup, one horizontal seed, recovery.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyHealth), typeof(EnemyHitStun))]
    public sealed class PepperRangedAttack : MonoBehaviour
    {
        [SerializeField] private Transform _facingRoot;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private SpriteRenderer _chargeIndicator;
        [SerializeField] private PepperSeedProjectile _projectilePrefab;
        [SerializeField] private LayerMask _groundLayers;
        [SerializeField, Min(1f)] private float _detectionRange = 10f;
        [SerializeField, Min(0.2f)] private float _windupDuration = 0.8f;
        [SerializeField, Min(0.2f)] private float _recoveryDuration = 1.5f;

        private enum Phase { Ready, Windup, Recovery }
        private Phase _phase;
        private EnemyHealth _health;
        private EnemyHitStun _stun;
        private PeaRangedVisual _peaVisual;
        private PlayerHealth _target;
        private PlayerRespawn _targetRespawn;
        private Collider2D _targetCollider;
        private PepperSeedProjectile _activeSeed;
        private Vector3 _facingScale;
        private Vector3 _indicatorScale;
        private float _remaining;
        private int _direction = -1;
        private bool _initialized;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _stun = GetComponent<EnemyHitStun>();
            _peaVisual = GetComponent<PeaRangedVisual>();
            _target = FindAnyObjectByType<PlayerHealth>();
            if (_facingRoot == null || _muzzle == null || _chargeIndicator == null ||
                _projectilePrefab == null || _target == null)
            {
                Debug.LogError($"{nameof(PepperRangedAttack)} needs its face, muzzle, warning, seed prefab and player.", this);
                enabled = false;
                return;
            }
            _targetRespawn = _target.GetComponent<PlayerRespawn>();
            _targetCollider = _target.GetComponent<Collider2D>();
            _facingScale = _facingRoot.localScale;
            _facingScale.x = Mathf.Abs(_facingScale.x);
            _indicatorScale = _chargeIndicator.transform.localScale;
            _initialized = true;
        }

        private void OnEnable()
        {
            if (!_initialized) return;
            _stun.StunStarted += OnStunStarted;
            _target.Died += OnPlayerUnavailable;
            _targetRespawn.Respawning += OnPlayerUnavailable;
            BeginRecovery();
        }

        private void FixedUpdate()
        {
            if (!_initialized || _health.CurrentHealth <= 0) return;
            if (_target == null || !_target.isActiveAndEnabled || _target.IsDead || !_targetRespawn.CanInteract)
            {
                BeginRecovery();
                return;
            }
            if (_stun.IsStunned) return;
            if (_phase == Phase.Recovery)
            {
                _remaining -= Time.fixedDeltaTime;
                if (_remaining <= 0f) _phase = Phase.Ready;
                return;
            }
            if (_phase == Phase.Ready)
            {
                if (_activeSeed != null || !CanSeeTarget()) return;
                _direction = _target.transform.position.x < transform.position.x ? -1 : 1;
                _facingRoot.localScale = new Vector3(_facingScale.x * _direction, _facingScale.y, _facingScale.z);
                _phase = Phase.Windup;
                _remaining = Mathf.Max(0.2f, _windupDuration);
            }
            // Facing is locked for this entire tell and shot. No tracking missiles.
            if (!CanSeeTarget()) { BeginRecovery(); return; }
            _remaining -= Time.fixedDeltaTime;
            float progress = 1f - Mathf.Clamp01(_remaining / Mathf.Max(0.2f, _windupDuration));
            _peaVisual?.ShowWindup(progress, _direction);
            _chargeIndicator.enabled = true;
            _chargeIndicator.transform.localScale = _indicatorScale * Mathf.Lerp(0.6f, 1.5f, progress);
            _chargeIndicator.color = Color.Lerp(new Color(1f, 0.8f, 0.1f), new Color(1f, 0.25f, 0.05f), progress);
            if (_remaining > 0f) return;
            _peaVisual?.ShowShot(_direction);
            _activeSeed = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(_activeSeed.gameObject, gameObject.scene);
            _activeSeed.Initialize(_direction, _target, _groundLayers);
            BeginRecovery(true);
        }

        private bool CanSeeTarget()
        {
            Vector2 delta = _target.transform.position - transform.position;
            if (Mathf.Abs(delta.x) > _detectionRange || Mathf.Abs(delta.y) > 3.5f) return false;
            // Sight is at eye level; low cover blocks the lower seed, not sight.
            Vector2 eye = (Vector2)transform.position + Vector2.up * 0.8f;
            Vector2 aim = _targetCollider != null ? (Vector2)_targetCollider.bounds.center : (Vector2)_target.transform.position;
            return Physics2D.Linecast(eye, aim, _groundLayers).collider == null;
        }

        private void OnStunStarted()
        {
            BeginRecovery();
            // The stun itself is the recovery penalty. Do not stack a second
            // post-shot cooldown on top; retain the full visible shot windup.
            _remaining = 0f;
        }

        private void OnPlayerUnavailable()
        {
            BeginRecovery();
            if (_activeSeed != null) _activeSeed.Despawn();
        }

        private void BeginRecovery(bool fired = false)
        {
            _peaVisual?.BeginRecovery(fired);
            _phase = Phase.Recovery;
            _remaining = Mathf.Max(0.2f, _recoveryDuration);
            if (_chargeIndicator != null)
            {
                _chargeIndicator.enabled = false;
                _chargeIndicator.transform.localScale = _indicatorScale;
            }
        }

        private void OnDisable()
        {
            if (!_initialized) return;
            _stun.StunStarted -= OnStunStarted;
            if (_target != null) _target.Died -= OnPlayerUnavailable;
            if (_targetRespawn != null) _targetRespawn.Respawning -= OnPlayerUnavailable;
            OnPlayerUnavailable();
        }
    }
}

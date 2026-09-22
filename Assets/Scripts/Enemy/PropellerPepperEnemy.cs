using System.Collections.Generic;
using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>Hover, telegraph, committed dive, vulnerable landing, takeoff. No ground chase.</summary>
    [DisallowMultipleComponent, RequireComponent(typeof(EnemyHealth), typeof(Rigidbody2D))]
    public sealed class PropellerPepperEnemy : MonoBehaviour
    {
        private enum Phase { Hover, Windup, Dive, Stuck, Takeoff }
        [SerializeField] private SpriteRenderer _visual;
        [SerializeField] private Transform _presentation;
        [SerializeField] private BoxCollider2D _visualHurtbox;
        [SerializeField] private Sprite[] _poses = new Sprite[9];
        [SerializeField] private Transform _leftLimit, _rightLimit;
        [SerializeField] private LayerMask _solids = 8;
        [SerializeField] private float _detectRange = 8.5f;
        [SerializeField] private float _hoverSpeed = 2.2f;
        [SerializeField] private float _windup = .5f;
        [SerializeField] private float _diveSpeed = 10f;
        [SerializeField] private float _missRecovery = .8f;
        [SerializeField] private float _hitRecovery = .45f;
        [SerializeField] private float _takeoffSpeed = 4.5f;
        [SerializeField] private float _betweenDives = .65f;
        private Rigidbody2D _body;
        private CapsuleCollider2D _hurtbox;
        private EnemyHealth _health;
        private EnemyHitStun _stun;
        private PlayerHealth _player;
        private Collider2D _playerBody;
        private Camera _camera;
        private Vector2 _home, _diveDirection;
        private Phase _phase;
        private float _until, _readyAt, _hurtUntil, _visualClock;
        private float _nextContactAt, _takeoffContactAt;
        private const float HurtVisualDuration = .36f;
        private int _facing = -1;
        private bool _hitThisDive, _landedHit;
        private ContactFilter2D _solidFilter, _contactFilter;
        private readonly List<RaycastHit2D> _hits = new(16);

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _hurtbox = GetComponent<CapsuleCollider2D>();
            _health = GetComponent<EnemyHealth>();
            _stun = GetComponent<EnemyHitStun>();
            _player = FindFirstObjectByType<PlayerHealth>();
            if (_player != null) _playerBody = _player.GetComponent<Collider2D>();
            _camera = Camera.main;
            _home = _body.position;
            _solidFilter = new ContactFilter2D();
            _solidFilter.SetLayerMask(_solids); _solidFilter.useTriggers = false;
            _contactFilter = new ContactFilter2D { useTriggers = true };
        }

        private void OnEnable()
        {
            ResetState();
            _health.Damaged += OnDamaged; _health.Died += OnDied; _health.Restored += ResetState;
            if (_stun != null) _stun.StunStarted += OnStunned;
        }

        private void OnDisable()
        {
            if (_health != null)
            { _health.Damaged -= OnDamaged; _health.Died -= OnDied; _health.Restored -= ResetState; }
            if (_stun != null) _stun.StunStarted -= OnStunned;
        }

        private void ResetState()
        {
            _phase = Phase.Hover; _facing = -1;
            _until = _hurtUntil = _visualClock = 0f;
            _nextContactAt = _takeoffContactAt = 0f;
            _readyAt = Time.time + _betweenDives;
            _hitThisDive = _landedHit = false;
            if (_body != null) { _body.position = _home; StopMotion(); }
            if (_presentation != null) _presentation.localRotation = Quaternion.identity;
            if (_visual != null && _poses != null && _poses.Length > 0)
            {
                _visual.sprite = _poses[0];
                _visual.flipX = true;
                UpdateVisualHurtbox(0);
            }
        }

        private bool OnScreen()
        {
            if (_camera == null) return true;
            Vector3 p = _camera.WorldToViewportPoint(transform.position);
            return p.z > 0f && p.x > .03f && p.x < .97f && p.y > .03f && p.y < .97f;
        }

        private bool CanSeePlayer()
        {
            if (_player == null || _player.IsDead || !_player.isActiveAndEnabled || _playerBody == null || !OnScreen()) return false;
            float playerX = _playerBody.bounds.center.x;
            if ((_leftLimit != null && playerX < _leftLimit.position.x) ||
                (_rightLimit != null && playerX > _rightLimit.position.x)) return false;
            Vector2 delta = (Vector2)_playerBody.bounds.center - _body.position;
            return delta.sqrMagnitude <= _detectRange * _detectRange &&
                !Physics2D.Raycast(_body.position, delta.normalized, delta.magnitude, _solids);
        }

        private void FixedUpdate()
        {
            StopMotion();
            if (_hurtbox == null || _player == null || _player.IsDead) return;
            // The natural recovery clock keeps running during hits; no endless stun banking.
            if (_stun != null && _stun.IsStunned) return;
            QueryAirContact();
            if (_player.IsDead || !isActiveAndEnabled) return;
            switch (_phase)
            {
                case Phase.Hover: Hover(); break;
                case Phase.Windup:
                    if (!CanSeePlayer()) { BeginTakeoff(); break; }
                    if (Time.time >= _until) BeginDive();
                    break;
                case Phase.Dive: Dive(); break;
                case Phase.Stuck:
                    if (Time.time >= _until) BeginTakeoff();
                    break;
                case Phase.Takeoff: Takeoff(); break;
            }
        }

        private void Hover()
        {
            if (!CanSeePlayer()) return;
            float dx = _playerBody.bounds.center.x - _body.position.x;
            _facing = dx < 0f ? -1 : 1;
            float left = _leftLimit != null ? _leftLimit.position.x : _home.x - 9f;
            float right = _rightLimit != null ? _rightLimit.position.x : _home.x + 9f;
            // Keep a diagonal approach rather than hovering directly on the chef.
            float desiredX = Mathf.Clamp(_playerBody.bounds.center.x - _facing * 3.5f, left, right);
            Vector2 target = new(desiredX, _home.y + Mathf.Sin(_visualClock * 3f) * .12f);
            Vector2 delta = Vector2.ClampMagnitude(target - _body.position, _hoverSpeed * Time.fixedDeltaTime);
            MoveSafely(delta, out _);
            if (Time.time < _readyAt || Mathf.Abs(dx) < 1.5f || Mathf.Abs(dx) > 5f ||
                _playerBody.bounds.center.y > _body.position.y - .5f) return;
            _phase = Phase.Windup; _until = Time.time + _windup;
        }

        private void BeginDive()
        {
            Vector2 delta = (Vector2)_playerBody.bounds.center - _body.position;
            delta.y = Mathf.Min(delta.y, -.8f);
            if (Mathf.Abs(delta.x) < .5f) delta.x = _facing * .5f;
            _diveDirection = delta.normalized;
            _facing = _diveDirection.x < 0f ? -1 : 1;
            _phase = Phase.Dive; _until = Time.time + 1.4f;
            _hitThisDive = _landedHit = false;
        }

        private void Dive()
        {
            Vector2 wanted = _diveDirection * (_diveSpeed * Time.fixedDeltaTime);
            float travel = SafeDistance(wanted, out Vector2 normal);
            if (OnScreen()) QueryDiveHit(_diveDirection, travel);
            if (!isActiveAndEnabled) return;
            _body.MovePosition(_body.position + _diveDirection * travel);
            if (travel + .001f < wanted.magnitude)
            {
                if (normal.y > .5f)
                { _phase = Phase.Stuck; _until = Time.time + (_landedHit ? _hitRecovery : _missRecovery); }
                else BeginTakeoff(); // Wall/ceiling is not a fake midair ground landing.
            }
            else if (Time.time >= _until) BeginTakeoff();
        }

        private void QueryDiveHit(Vector2 direction, float distance)
        {
            if (_hitThisDive || _playerBody == null || _player.IsDead || Time.time < _nextContactAt) return;
            bool hit = _hurtbox.Distance(_playerBody).isOverlapped;
            int count = _hurtbox.Cast(direction, _contactFilter, _hits, distance);
            for (int i = 0; i < count && !hit; i++)
                hit = _hits[i].collider != null && _hits[i].collider.GetComponentInParent<PlayerHealth>() == _player;
            if (!hit) return;
            _hitThisDive = true;
            int before = _player.CurrentHealth;
            _player.TakeDamage(1);
            _landedHit = _player != null && _player.CurrentHealth < before;
            if (_landedHit) _nextContactAt = Time.time + .8f;
        }

        private void QueryAirContact()
        {
            // Dive keeps its existing swept attack area. The stuck counterattack
            // window and the first instant of lift-off remain safe to approach.
            if (_phase == Phase.Dive || _phase == Phase.Stuck ||
                (_phase == Phase.Takeoff && Time.time < _takeoffContactAt) ||
                Time.time < _nextContactAt || _playerBody == null || !OnScreen()) return;
            bool touching = _hurtbox.Distance(_playerBody).isOverlapped;
            if (!touching && _visualHurtbox != null && _visualHurtbox.enabled)
                touching = _visualHurtbox.Distance(_playerBody).isOverlapped;
            if (!touching) return;
            int before = _player.CurrentHealth;
            _player.TakeDamage(1); // Retains dash, respawn and normal hurt protection.
            if (_player.CurrentHealth < before) _nextContactAt = Time.time + .8f;
        }

        private void BeginTakeoff()
        {
            _phase = Phase.Takeoff; _until = Time.time + 1.5f;
            _takeoffContactAt = Time.time + .2f;
        }

        private void Takeoff()
        {
            Vector2 delta = Vector2.up * Mathf.Clamp(_home.y - _body.position.y, 0f, _takeoffSpeed * Time.fixedDeltaTime);
            bool clear = MoveSafely(delta, out _);
            if (_body.position.y >= _home.y - .06f || !clear || Time.time >= _until)
            { _phase = Phase.Hover; _readyAt = Time.time + _betweenDives; }
        }

        private bool MoveSafely(Vector2 delta, out Vector2 normal)
        {
            float distance = SafeDistance(delta, out normal);
            if (delta.sqrMagnitude > .000001f) _body.MovePosition(_body.position + delta.normalized * distance);
            return distance + .001f >= delta.magnitude;
        }

        private float SafeDistance(Vector2 delta, out Vector2 normal)
        {
            normal = Vector2.zero;
            float distance = delta.magnitude;
            if (distance <= .00001f) return 0f;
            Vector2 direction = delta / distance;
            int count = _hurtbox.Cast(direction, _solidFilter, _hits, distance + .025f);
            for (int i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (hit.collider == null || hit.collider.attachedRigidbody == _body ||
                    Vector2.Dot(hit.normal, direction) >= -.01f) continue;
                float safe = Mathf.Max(0f, hit.distance - .025f);
                if (safe <= distance) { distance = safe; normal = hit.normal; }
            }
            return distance;
        }

        private void StopMotion()
        {
            if (_body == null) return;
            _body.linearVelocity = Vector2.zero; _body.MovePosition(_body.position);
        }

        private void OnDamaged() => _hurtUntil = Time.time + HurtVisualDuration;
        private void OnStunned()
        {
            if (_phase == Phase.Dive || _phase == Phase.Windup) BeginTakeoff();
            StopMotion();
        }

        private void LateUpdate()
        {
            if (_visual == null || _poses == null || _poses.Length < 9) return;
            _visualClock = Mathf.Repeat(_visualClock + Time.deltaTime, 100f);
            int index = _phase switch
            {
                Phase.Windup => 3, Phase.Dive => 4, Phase.Stuck => 5,
                Phase.Takeoff => 6, _ => Mathf.Clamp((int)(_visualClock * 10f) % 3, 0, 2)
            };
            if (Time.time < _hurtUntil) index = 7;
            if (_poses[index] != null) _visual.sprite = _poses[index];
            _visual.flipX = _facing < 0;
            if (_presentation != null)
            {
                float tilt = _phase == Phase.Windup ? _facing * 10f *
                    Mathf.Clamp01(1f - (_until - Time.time) / _windup) :
                    _phase == Phase.Dive ? -_facing * 25f :
                    _phase == Phase.Stuck ? Mathf.Sin(_visualClock * 22f) * 2f : 0f;
                if (Time.time < _hurtUntil)
                {
                    // Clear recoil, brief held pain pose, then damped recovery.
                    // Presentation only: forks do not acquire gameplay stun.
                    float progress = Mathf.Clamp01(1f - (_hurtUntil - Time.time) / HurtVisualDuration);
                    float recoil = progress < .2f ? Mathf.Sin(progress / .2f * Mathf.PI * .5f) :
                        progress < .5f ? 1f : Mathf.Cos((progress - .5f) * Mathf.PI);
                    tilt = -_facing * 20f * recoil;
                    tilt += Mathf.Sin(progress * Mathf.PI * 4f) * 4f * (1f - progress);
                }
                _presentation.localRotation = Quaternion.Euler(0f, 0f, tilt);
            }
            UpdateVisualHurtbox(index);
        }

        // Receiving damage follows the painted body, independently of the original
        // root capsule used for dive damage and wall/floor clearance. Exclude the rotor.
        private void UpdateVisualHurtbox(int pose)
        {
            if (_visualHurtbox == null || _visual == null || _visual.sprite == null) return;
            Rect body = pose == 4 ? new Rect(.14f, .08f, .80f, .58f) :
                pose == 5 ? new Rect(.27f, .04f, .46f, .77f) :
                new Rect(.18f, .10f, .60f, .62f);
            Bounds bounds = _visual.sprite.bounds;
            Vector2 size = Vector2.Scale(bounds.size, body.size);
            Vector2 center = (Vector2)bounds.min + Vector2.Scale(bounds.size, body.center);
            if (_visual.flipX) center.x = -center.x;
            if (_visual.flipY) center.y = -center.y;
            _visualHurtbox.size = size;
            _visualHurtbox.offset = center;
        }

        private void OnDied()
        {
            if (_visual == null || _poses == null || _poses.Length < 9 || _poses[8] == null) return;
            var effect = new GameObject("PropellerPepper_Defeated");
            effect.transform.SetParent(transform.parent, false);
            effect.transform.position = transform.position;
            var sprite = effect.AddComponent<SpriteRenderer>();
            sprite.sprite = _poses[8]; sprite.sharedMaterial = _visual.sharedMaterial;
            sprite.sortingLayerID = _visual.sortingLayerID; sprite.sortingOrder = _visual.sortingOrder;
            sprite.flipX = _facing < 0;
            effect.AddComponent<PropellerPepperDefeat>().Initialize(_solids);
        }
    }
}

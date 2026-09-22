using System.Collections.Generic;
using KitchenChaos.Player;
using UnityEngine;
namespace KitchenChaos.Enemy
{
    [RequireComponent(typeof(EnemyHealth), typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class BellPepperBomber : MonoBehaviour
    {
        private enum Phase { Ready, Retreat, Windup, Release, Recovery }
        [SerializeField] private SpriteRenderer _visual;
        [SerializeField] private Sprite[] _poses = new Sprite[16];
        [SerializeField] private Transform _leftLimit, _rightLimit;
        [SerializeField] private LayerMask _solids = 8;
        [SerializeField] private float _detectRange = 9f, _walkSpeed = 1.2f, _retreatSpeed = 2.8f;
        [SerializeField] private float _windup = .8f, _recovery = .65f;
        private Rigidbody2D _body;
        private BoxCollider2D _collider;
        private EnemyHealth _health;
        private EnemyHitStun _stun;
        private PlayerHealth _player;
        private PlayerRespawn _respawn;
        private Collider2D _playerBody;
        private Camera _camera;
        private BellPepperBomb _bomb;
        private ContactFilter2D _filter;
        private readonly List<RaycastHit2D> _hits = new(8);
        private Phase _phase;
        private float _until, _hurtUntil, _walkClock, _lastX;
        private int _facing = -1, _patrolDirection = -1;
        private bool _alerted, _retreated;
        private Vector2 _home, _aim;
        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>(); _collider = GetComponent<BoxCollider2D>();
            _health = GetComponent<EnemyHealth>(); _stun = GetComponent<EnemyHitStun>();
            _player = FindFirstObjectByType<PlayerHealth>();
            if (_player != null) { _playerBody = _player.GetComponent<Collider2D>(); _respawn = _player.GetComponent<PlayerRespawn>(); }
            _camera = Camera.main; _home = _body.position;
            _filter = new ContactFilter2D { useTriggers = false }; _filter.SetLayerMask(_solids);
        }
        private void OnEnable()
        {
            ResetState(); _health.Damaged += OnDamaged; _health.Died += OnDied; _health.Restored += ResetState;
            if (_stun != null) _stun.StunStarted += OnStunned;
            if (_player != null) _player.Died += ClearBomb;
            if (_respawn != null) _respawn.Respawning += ClearBomb;
        }
        private void OnDisable()
        {
            if (_health != null) { _health.Damaged -= OnDamaged; _health.Died -= OnDied; _health.Restored -= ResetState; }
            if (_stun != null) _stun.StunStarted -= OnStunned;
            if (_player != null) _player.Died -= ClearBomb;
            if (_respawn != null) _respawn.Respawning -= ClearBomb;
            ClearBomb();
        }
        private void ClearBomb() { if (_bomb != null) { _bomb.gameObject.SetActive(false); Destroy(_bomb.gameObject); } _bomb = null; }
        private void ResetState()
        {
            ClearBomb(); _phase = Phase.Ready; _until = Time.time + .5f;
            _alerted = _retreated = false; _facing = _patrolDirection = -1;
            _hurtUntil = _walkClock = 0; _lastX = _home.x;
            if (_body != null) _body.position = _home;
        }
        private bool Visible()
        {
            if (_player == null || _player.IsDead || !_player.isActiveAndEnabled || _playerBody == null) return false;
            if (_camera != null)
            { Vector3 v = _camera.WorldToViewportPoint(transform.position); if (v.z <= 0 || v.x < .04f || v.x > .96f || v.y < 0 || v.y > 1) return false; }
            Vector2 delta = (Vector2)_playerBody.bounds.center - _body.position;
            return delta.sqrMagnitude <= _detectRange * _detectRange && !Physics2D.Raycast(_body.position, delta.normalized, delta.magnitude, _solids);
        }
        private void FixedUpdate()
        {
            _body.linearVelocity = Vector2.zero; _body.MovePosition(_body.position);
            if (_player == null || _player.IsDead || (_stun != null && _stun.IsStunned)) return;
            bool visible = Visible(); if (visible) _alerted = true;
            if (_phase == Phase.Retreat)
            {
                if (!visible || Time.time >= _until || !Step(-_facing, _retreatSpeed * Time.fixedDeltaTime)) BeginWindup();
                return;
            }
            if (_phase == Phase.Windup)
            {
                if (!visible) { _phase = Phase.Recovery; _until = Time.time + _recovery; return; }
                if (Time.time >= _until) { Throw(); _phase = Phase.Release; _until = Time.time + .2f; }
                return;
            }
            if (_phase == Phase.Release || _phase == Phase.Recovery)
            {
                if (Time.time < _until) return;
                if (_phase == Phase.Release) { _phase = Phase.Recovery; _until = Time.time + _recovery; }
                else _phase = Phase.Ready;
                return;
            }
            if (!visible)
            {
                if (!_alerted) { _facing = _patrolDirection; if (!Step(_patrolDirection, _walkSpeed * Time.fixedDeltaTime)) _patrolDirection *= -1; }
                return;
            }
            _facing = _playerBody.bounds.center.x < _body.position.x ? -1 : 1;
            if (_bomb != null || Time.time < _until) return;
            if (!_retreated && Mathf.Abs(_playerBody.bounds.center.x - _body.position.x) < 3.5f)
            { _retreated = true; _phase = Phase.Retreat; _until = Time.time + .4f; }
            else BeginWindup();
        }
        private void BeginWindup()
        {
            _phase = Phase.Windup; _until = Time.time + _windup;
            if (_playerBody != null)
            { _facing = _playerBody.bounds.center.x < _body.position.x ? -1 : 1; _aim = new Vector2(_playerBody.bounds.center.x, _playerBody.bounds.min.y + .2f); }
        }
        private void Throw()
        {
            if (_bomb != null || _poses.Length < 16) return;
            // Authored release hand: forward/up relative to the foot-pivot visual.
            Vector2 start = _body.position + new Vector2(_facing * 1.35f, .4f);
            Vector2 chest = _body.position;
            var obstruction = Physics2D.Linecast(chest, start, _solids);
            if (obstruction) start = chest;
            var go = new GameObject("BellPepper_SeedBomb"); go.transform.SetParent(transform.parent, false);
            _bomb = go.AddComponent<BellPepperBomb>();
            _bomb.Initialize(start, _aim, _poses[14], _poses[15], _visual.sharedMaterial,
                _visual.sortingLayerID, _visual.sortingOrder + 1, _solids, _player);
            _retreated = false;
        }
        private bool Step(int direction, float requested)
        {
            Bounds b = _collider.bounds; float distance = requested;
            float edge = direction < 0 ? _leftLimit.position.x : _rightLimit.position.x;
            distance = Mathf.Min(distance, Mathf.Max(0, (edge - _body.position.x) * direction));
            int n = Physics2D.BoxCast(b.center, b.size - new Vector3(.04f, .08f, 0), 0, Vector2.right * direction, _filter, _hits, distance + .04f);
            for (int i = 0; i < n; i++) if (_hits[i].collider != null && _hits[i].collider.attachedRigidbody != _body && _hits[i].normal.x * direction < -.5f)
                distance = Mathf.Min(distance, Mathf.Max(0, _hits[i].distance - .04f));
            Vector2 foot = new(b.center.x + direction * (b.extents.x + distance), b.min.y + .1f);
            if (!Physics2D.Raycast(foot, Vector2.down, .3f, _solids)) return false;
            _body.MovePosition(_body.position + Vector2.right * (direction * distance));
            return distance > .001f;
        }
        private void OnDamaged() { _hurtUntil = Time.time + .36f; _alerted = true; }
        private void OnStunned() { _phase = Phase.Recovery; _until = Time.time + .3f; }
        private void LateUpdate()
        {
            if (_visual == null || _poses.Length < 16) return;
            float moved = Mathf.Abs(_body.position.x - _lastX); _lastX = _body.position.x;
            _walkClock = Mathf.Repeat(_walkClock + moved / 1.0f, 1f);
            int pose = moved > .0005f ? 1 + Mathf.Clamp((int)(_walkClock * 4), 0, 3) : 0;
            if (_phase == Phase.Windup) pose = _until - Time.time > _windup * .5f ? 5 : 6;
            else if (_phase == Phase.Release) pose = 7;
            else if (_phase == Phase.Recovery) pose = 8;
            if (Time.time < _hurtUntil) pose = _hurtUntil - Time.time > .18f ? 9 : 10;
            _visual.sprite = _poses[pose]; _visual.flipX = _facing < 0;
        }
        private void OnDied()
        {
            ClearBomb(); var go = new GameObject("BellPepper_Defeated"); go.transform.SetParent(transform.parent, false);
            go.transform.position = transform.position + Vector3.down * 1.45f;
            var visual = go.AddComponent<SpriteRenderer>(); visual.sprite = _poses[11]; visual.flipX = _facing < 0;
            visual.sharedMaterial = _visual.sharedMaterial; visual.sortingLayerID = _visual.sortingLayerID; visual.sortingOrder = _visual.sortingOrder;
            go.AddComponent<BellPepperDefeat>().Initialize(visual, _poses);
        }
    }
}

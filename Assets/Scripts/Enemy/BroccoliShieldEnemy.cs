using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>Advancing shield guard; flanking is safer than its brief frontal opening.</summary>
    [DisallowMultipleComponent, RequireComponent(typeof(EnemyHealth))]
    public sealed class BroccoliShieldEnemy : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _visual;
        [SerializeField] private Transform _presentation;
        [SerializeField] private Sprite[] _poses = new Sprite[6];
        [SerializeField] private Sprite[] _walkFrames;
        [SerializeField] private Sprite[] _parryFrames;
        private float _walkTravel, _lastVisualX, _parryVisualStarted = -1000f;
        private const float WalkCycleDistance = 1.2f;
        private const float ParryVisualDuration = .24f;
        [SerializeField] private LayerMask _solids = 8;
        [SerializeField] private int _initialFacing = -1;
        [SerializeField, Min(.1f)] private float _windup = .35f;
        [SerializeField, Min(.05f)] private float _activeTime = .16f;
        [SerializeField, Min(.1f)] private float _recovery = .15f;
        [SerializeField, Min(.1f)] private float _guardPause = .5f;
        [SerializeField, Min(.1f)] private float _turnDelay = .6f;
        [SerializeField, Min(.1f)] private float _triggerRange = 2.2f;
        [SerializeField, Min(.1f)] private float _bashReach = 2.2f;
        [SerializeField, Min(0f)] private float _approachSpeed = 2f;
        [SerializeField, Min(0f)] private float _bashSpeed = 6f;
        private Rigidbody2D _body;
        private Collider2D _hurtbox;
        private ContactFilter2D _solidFilter;
        private readonly System.Collections.Generic.List<RaycastHit2D> _walls = new(8);
        private readonly System.Collections.Generic.List<RaycastHit2D> _ground = new(4);
        private bool _moving;
        private EnemyHealth _health;
        private EnemyHitStun _stun;
        private PlayerHealth _player;
        private Collider2D _playerBody;
        private Camera _camera;
        private int _facing, _phase;
        private float _phaseUntil, _readyAt, _turnClock, _hurtUntil, _blockUntil;
        private bool _hitThisAttack;
        private float _parryPauseUntil;
        private bool _projectileParryVisual;

        // Hurt art and melee stun must not lengthen the frontal opening.
        public bool IsGuarding => isActiveAndEnabled && _phase != 3;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _stun = GetComponent<EnemyHitStun>();
            _body = GetComponent<Rigidbody2D>();
            _hurtbox = GetComponent<Collider2D>();
            _solidFilter = new ContactFilter2D();
            _solidFilter.SetLayerMask(_solids); _solidFilter.useTriggers = false;
            _player = FindFirstObjectByType<PlayerHealth>();
            if (_player != null) _playerBody = _player.GetComponent<Collider2D>();
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            ResetState();
            _health.Damaged += OnDamaged;
            _health.Died += OnDied;
            _health.Restored += ResetState;
            if (_stun != null) _stun.StunStarted += OnStunned;
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.Damaged -= OnDamaged; _health.Died -= OnDied;
                _health.Restored -= ResetState;
            }
            if (_stun != null) _stun.StunStarted -= OnStunned;
        }

        private void ResetState()
        {
            _facing = _initialFacing < 0 ? -1 : 1;
            _phase = 0; _readyAt = Time.time + _guardPause;
            _turnClock = _hurtUntil = _blockUntil = 0f;
            _moving = false;
            _hitThisAttack = false;
            _parryPauseUntil = 0f;
            _walkTravel = 0f; _lastVisualX = transform.position.x;
            _parryVisualStarted = -1000f;
            _projectileParryVisual = false;
        }

        public bool TryBlock(Vector2 source)
        {
            if (!IsGuarding || (source.x - transform.position.x) * _facing <= .05f) return false;
            _blockUntil = Time.time + .16f;
            return true;
        }

        public bool TryParryMelee(PlayerMobility attacker, Vector2 source)
        {
            // The committed bash still blocks normally; only defensive poses parry.
            if (attacker == null || !isActiveAndEnabled || (_phase != 0 && _phase != 1) ||
                (_stun != null && _stun.IsStunned) ||
                (source.x - transform.position.x) * _facing <= .05f) return false;
            float separation = _bashReach + _bashSpeed * _activeTime + .8f;
            if (!attacker.ReceiveShieldParry(transform.position, separation)) return false;
            _phase = 0; _hitThisAttack = false;
            _parryPauseUntil = Time.time + PlayerMobility.ParryStunDuration + .4f;
            _readyAt = Mathf.Max(_readyAt, _parryPauseUntil);
            _blockUntil = Time.time + .2f;
            _parryVisualStarted = Time.time;
            _projectileParryVisual = false;
            StopMotion();
            return true;
        }

        public bool TryReflectFork(Vector2 source)
        {
            if (!IsGuarding || (_stun != null && _stun.IsStunned) ||
                (source.x - transform.position.x) * _facing <= .05f) return false;
            // Reflection never cancels movement, resets attack clocks or extends
            // the melee parry pause: firing repeatedly cannot pin the guard.
            // A committed bash retains its readable attack pose over this effect.
            if (Time.time - _parryVisualStarted >= ParryVisualDuration)
            {
                _parryVisualStarted = Time.time;
                _projectileParryVisual = true;
            }
            return true;
        }

        private void OnDamaged() { _hurtUntil = Time.time + .18f; }
        private void OnStunned()
        {
            // A genuine melee stagger cancels the committed bash, not just its picture.
            // Interrupt the bash without restarting a rear-turn timer or exposing
            // the front for the whole stun. Natural recovery retains its deadline.
            if (_phase != 3) { _phase = 0; _readyAt = Time.time + _guardPause; }
            StopMotion();
        }

        private bool TargetVisible()
        {
            if (_player == null || _player.IsDead || _playerBody == null || !_player.isActiveAndEnabled) return false;
            if (_camera != null)
            {
                Vector3 v = _camera.WorldToViewportPoint(transform.position);
                if (v.z <= 0f || v.x < 0f || v.x > 1f || v.y < 0f || v.y > 1f) return false;
            }
            Vector2 delta = (Vector2)_playerBody.bounds.center - (Vector2)transform.position;
            return delta.sqrMagnitude < 64f && !Physics2D.Raycast(transform.position, delta.normalized, delta.magnitude, _solids);
        }

        private void FixedUpdate()
        {
            StopMotion();
            if (Time.time < _parryPauseUntil) return;
            bool visible = TargetVisible();
            float dx = visible ? _playerBody.bounds.center.x - transform.position.x : 0f;
            // Time spent behind counts during an attack/stun; hits never reset it.
            if (visible && dx * _facing < -.2f) _turnClock += Time.fixedDeltaTime;
            else _turnClock = 0f;
            if (_phase != 0)
            {
                if (Time.time >= _phaseUntil)
                {
                    _phase++;
                    if (_phase == 2) _phaseUntil = Time.time + _activeTime;
                    else if (_phase == 3) _phaseUntil = Time.time + _recovery;
                    else { _phase = 0; _readyAt = Time.time + _guardPause; }
                }
                if (_phase == 2 && visible && (_stun == null || !_stun.IsStunned))
                {
                    float step = SafeStep(_bashSpeed * Time.fixedDeltaTime);
                    QueryBash(step);
                    if (_health.CurrentHealth > 0) Move(step);
                }
                return;
            }
            if (!visible || (_stun != null && _stun.IsStunned)) return;
            if (dx * _facing < -.2f)
            {
                if (_turnClock >= _turnDelay) { _facing *= -1; _turnClock = 0f; }
                return;
            }
            _turnClock = 0f;
            float dy = Mathf.Abs(_playerBody.bounds.center.y - transform.position.y);
            if (Time.time >= _readyAt && Mathf.Abs(dx) <= _triggerRange && dy < 1.8f)
            {
                _phase = 1; _phaseUntil = Time.time + _windup;
                _hitThisAttack = false;
            }
            else if (Mathf.Abs(dx) > _triggerRange && dy < 3f)
                Move(SafeStep(Mathf.Min(_approachSpeed * Time.fixedDeltaTime, Mathf.Abs(dx) - _triggerRange)));
        }

        private void StopMotion()
        {
            _moving = false;
            if (_body == null) return;
            _body.linearVelocity = Vector2.zero;
            _body.MovePosition(_body.position);
        }

        private void Move(float distance)
        {
            if (_body == null || distance <= 0f) return;
            _moving = true;
            _body.MovePosition(_body.position + Vector2.right * (_facing * distance));
        }

        private float SafeStep(float distance)
        {
            if (_hurtbox == null || _body == null || distance <= 0f) return 0f;
            Bounds b = _hurtbox.bounds;
            Vector2 direction = Vector2.right * _facing;
            int count = Physics2D.BoxCast(b.center, new Vector2(b.size.x - .04f, b.size.y - .08f),
                0f, direction, _solidFilter, _walls, distance + .04f);
            for (int i = 0; i < count; i++)
                if (_walls[i].collider != null && _walls[i].collider.attachedRigidbody != _body &&
                    Vector2.Dot(_walls[i].normal, direction) < -.5f)
                    distance = Mathf.Min(distance, Mathf.Max(0f, _walls[i].distance - .04f));
            for (float sample = 0f; sample <= distance + .099f; sample += .1f)
            {
                float offset = Mathf.Min(sample, distance);
                Vector2 foot = new(b.center.x + _facing * (b.extents.x + offset), b.min.y + .1f);
                if (Physics2D.Raycast(foot, Vector2.down, _solidFilter, _ground, .3f) == 0)
                    return Mathf.Max(0f, offset - .1f);
            }
            return distance;
        }

        private void QueryBash(float step)
        {
            if (_hitThisAttack || _playerBody == null) return;
            float reach = _bashReach + step;
            Bounds hit = new Bounds(transform.position + Vector3.right * (_facing * reach * .5f),
                new Vector3(reach, 2.25f, 4f));
            if (!hit.Intersects(_playerBody.bounds)) return;
            _hitThisAttack = true;
            _player.TakeDamage(1);
        }

        private void LateUpdate()
        {
            if (_visual == null) return;
            float distance = Mathf.Abs(transform.position.x - _lastVisualX);
            _lastVisualX = transform.position.x;
            bool walking = _moving && _phase == 0 && Time.time >= _parryPauseUntil &&
                (_stun == null || !_stun.IsStunned);
            if (walking) _walkTravel = Mathf.Repeat(_walkTravel + distance, WalkCycleDistance);
            else _walkTravel = 0f;
            // Keep the lid in front during protected preparation rather than
            // showing the old raised-lid/exposed-torso pose.
            int pose = Time.time < _hurtUntil ? 4 : (_phase == 1 ? 0 : _phase);
            if (_poses != null && pose < _poses.Length && _poses[pose] != null) _visual.sprite = _poses[pose];
            bool parrying = Time.time - _parryVisualStarted < ParryVisualDuration &&
                (!_projectileParryVisual || (_phase != 2 && _phase != 3));
            if (Time.time >= _hurtUntil)
            {
                if (parrying && HasFrames(_parryFrames))
                {
                    float progress = Mathf.Clamp01((Time.time - _parryVisualStarted) / ParryVisualDuration);
                    // A brief contact pose, a sharp upward sweep, then a readable
                    // follow-through. This visual clock never delays the recoil.
                    int frame = _parryFrames.Length == 4
                        ? (progress < .125f ? 0 : progress < .42f ? 1 : progress < .75f ? 2 : 3)
                        : Mathf.Clamp((int)(progress * _parryFrames.Length), 0, _parryFrames.Length - 1);
                    _visual.sprite = _parryFrames[frame];
                }
                else if (walking && HasFrames(_walkFrames))
                    _visual.sprite = _walkFrames[Mathf.Clamp((int)(_walkTravel / WalkCycleDistance * _walkFrames.Length), 0, _walkFrames.Length - 1)];
            }
            _visual.flipX = _facing < 0;
            if (_presentation != null)
            {
                float remaining = Mathf.Max(0f, _blockUntil - Time.time);
                float tilt = _phase == 1 ? -_facing * 9f *
                    Mathf.Clamp01(1f - (_phaseUntil - Time.time) / _windup) :
                    0f;
                _presentation.localRotation = Quaternion.Euler(0f, 0f, tilt +
                    (!parrying && remaining > 0f ? Mathf.Sin(remaining * 70f) * 5f * remaining / .16f : 0f));
            }
        }

        private static bool HasFrames(Sprite[] frames)
        {
            if (frames == null || frames.Length == 0) return false;
            foreach (var frame in frames) if (frame == null) return false;
            return true;
        }

        private void OnDied()
        {
            if (_visual == null || _poses == null || _poses.Length < 6 || _poses[5] == null) return;
            // Parent belongs to the reset entry: retry destroys even a lingering defeat visual.
            var fallen = new GameObject("Broccoli_Defeated");
            fallen.transform.SetParent(transform.parent, false);
            fallen.transform.position = new Vector3(transform.position.x, _visual.transform.position.y, transform.position.z);
            var sprite = fallen.AddComponent<SpriteRenderer>();
            sprite.sprite = _poses[5]; sprite.sharedMaterial = _visual.sharedMaterial;
            sprite.sortingLayerID = _visual.sortingLayerID; sprite.sortingOrder = _visual.sortingOrder;
            sprite.flipX = _facing < 0;
            fallen.AddComponent<BroccoliDefeatPlayback>();
        }
    }
}

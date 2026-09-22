using System.Collections.Generic;
using KitchenChaos.Enemy;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>Swept finite-range fork; a guarded reflection changes its damage target once.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ForkProjectile : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float _speed = 14f;
        [SerializeField, Min(0.1f)] private float _range = 8f;
        [SerializeField, Min(0.01f)] private float _radius = 0.1f;
        [SerializeField, Min(1)] private int _damage = 1;
        [SerializeField] private SpriteRenderer[] _sparks;
        private PlayerRangedAttack _owner;
        private SpriteRenderer _renderer;
        private Vector2 _direction;
        private LayerMask _solids;
        private float _remaining, _impactRemaining;
        private bool _initialized, _spent;
        private bool _reflected;
        private PlayerHealth _playerTarget;
        private float _returnLaunchAt;
        private const float ReturnSpeed = 9.5f;
        private const float DeflectionHold = .08f;
        private readonly List<RaycastHit2D> _hits = new(16);
        private readonly List<Collider2D> _overlaps = new(16);
        private ContactFilter2D _filter;

        public void Initialize(PlayerRangedAttack owner, Vector2 direction, LayerMask solids, Vector2 chest, Vector2 hand)
        {
            _owner = owner; _direction = direction.normalized; _solids = solids;
            _playerTarget = owner.GetComponent<PlayerHealth>();
            _renderer = GetComponent<SpriteRenderer>(); _remaining = _range;
            _filter = new ContactFilter2D { useTriggers = true };
            _initialized = true;
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            foreach (var spark in _sparks) if (spark != null) spark.enabled = false;
            Vector2 launch = hand + direction;
            Vector2 delta = launch - chest;
            Sweep(chest, delta.normalized, delta.magnitude);
        }

        private void FixedUpdate()
        {
            if (!_initialized || _spent) return;
            if (_owner == null || !_owner.isActiveAndEnabled || _remaining <= 0f) { Despawn(); return; }
            if (_reflected && Time.time < _returnLaunchAt) return;
            Sweep(transform.position, _direction, Mathf.Min((_reflected ? ReturnSpeed : _speed) * Time.fixedDeltaTime, _remaining));
        }

        private bool Eligible(Collider2D collider, out EnemyHealth enemy, out PlayerHealth player)
        {
            enemy = null; player = null;
            if (collider == null) return false;
            if (!collider.isTrigger && (_solids.value & (1 << collider.gameObject.layer)) != 0) return true;
            if (_reflected)
            {
                player = collider.GetComponentInParent<PlayerHealth>();
                return player != null && player == _playerTarget && player.isActiveAndEnabled && !player.IsDead;
            }
            enemy = collider.GetComponentInParent<EnemyHealth>();
            return enemy != null && enemy.isActiveAndEnabled && enemy.CurrentHealth > 0;
        }

        private void Sweep(Vector2 start, Vector2 direction, float distance)
        {
            // Explicit overlap handles starts inside a hurtbox/solid regardless of
            // the global Physics2D.queriesStartInColliders setting.
            int overlaps = Physics2D.OverlapCircle(start, _radius, _filter, _overlaps);
            EnemyHealth overlappingEnemy = null;
            PlayerHealth overlappingPlayer = null;
            for (int i = 0; i < overlaps; i++)
            {
                if (!Eligible(_overlaps[i], out var e, out var p)) continue;
                if (e == null && p == null) { Impact(start, null, null); return; }
                overlappingEnemy = e; overlappingPlayer = p;
            }
            if (overlappingEnemy != null || overlappingPlayer != null)
            { Impact(start, overlappingEnemy, overlappingPlayer); return; }
            int count = Physics2D.CircleCast(start, _radius, direction, _filter, _hits, distance);
            float closest = float.PositiveInfinity; EnemyHealth victim = null; bool blocked = false;
            PlayerHealth playerVictim = null;
            for (int i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (hit.distance > closest || !Eligible(hit.collider, out var e, out var p)) continue;
                // At equal distance a solid wins over an overlapping hurtbox.
                if (blocked && Mathf.Approximately(hit.distance, closest) && victim == null && playerVictim == null) continue;
                closest = hit.distance; victim = e; playerVictim = p; blocked = true;
            }
            if (blocked) { Impact(start + direction * closest, victim, playerVictim); return; }
            transform.position = start + direction * distance;
            _remaining -= distance;
        }

        private void Impact(Vector2 point, EnemyHealth victim, PlayerHealth player)
        {
            if (!_reflected && victim != null && _playerTarget != null && !_playerTarget.IsDead)
            {
                var shield = victim.GetComponent<BroccoliShieldEnemy>();
                if (shield != null && shield.TryReflectFork(point - _direction * .2f))
                { Reflect(point); return; }
            }
            _spent = true; _renderer.enabled = false; transform.position = point;
            _impactRemaining = 0.14f;
            foreach (var spark in _sparks) if (spark != null) spark.enabled = true;
            // Establish spent state before health callbacks: a fatal hit can
            // synchronously despawn all shots through the owner's death event.
            if (player != null) { player.TakeDamage(1); return; }
            if (victim != null)
            {
                int before = victim.CurrentHealth;
                victim.TakeDamage(_damage, false, point - _direction * .2f);
                if (victim != null && victim.CurrentHealth < before && _owner != null) _owner.ReportHit(point);
            }
        }

        private void Reflect(Vector2 point)
        {
            // Reverse the authored shot angle for every stance and aim direction.
            // Never retarget the chef or alter the return height to aid dodging.
            _direction = -_direction;
            _reflected = true;
            _remaining = _range;
            _returnLaunchAt = Time.time + DeflectionHold;
            transform.SetPositionAndRotation(point, Quaternion.Euler(0f, 0f,
                Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg));
            // Remain registered with the original owner so death/respawn/disable
            // also clears hostile returns. Do not report hits or grant energy.
        }

        private void Update()
        {
            if (!_spent) return;
            _impactRemaining -= Time.deltaTime;
            if (_impactRemaining <= 0f) { Despawn(); return; }
            foreach (var spark in _sparks) if (spark != null)
            {
                var color = spark.color; color.a = _impactRemaining / 0.14f; spark.color = color;
            }
        }

        public void Despawn() { if (_owner != null) _owner.Forget(this); gameObject.SetActive(false); Destroy(gameObject); }
        private void OnDestroy() { if (_owner != null) _owner.Forget(this); }
    }
}

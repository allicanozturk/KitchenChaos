using System.Collections.Generic;
using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>Swept collision: no tunnelling and no repeated trigger damage.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PepperSeedProjectile : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float _speed = 7.5f;
        [SerializeField, Min(0.01f)] private float _radius = 0.16f;
        [SerializeField, Min(0.1f)] private float _lifetime = 3f;
        [SerializeField, Min(1)] private int _damage = 1;
        private readonly List<RaycastHit2D> _hits = new(8);
        private ContactFilter2D _filter;
        private LayerMask _groundLayers;
        private PlayerHealth _target;
        private PlayerRespawn _respawn;
        private Vector2 _direction;
        private float _remaining;
        private bool _initialized;
        private bool _spent;

        public void Initialize(int direction, PlayerHealth target, LayerMask groundLayers)
        {
            _target = target;
            _respawn = target.GetComponent<PlayerRespawn>();
            _groundLayers = groundLayers;
            _direction = direction < 0 ? Vector2.left : Vector2.right;
            _remaining = Mathf.Max(0.1f, _lifetime);
            // Other triggers (coins, checkpoints, hazards) never absorb a shot.
            _filter = new ContactFilter2D { useTriggers = true };
            _target.Died += Despawn;
            _respawn.Respawning += Despawn;
            _initialized = true;
        }

        private void FixedUpdate()
        {
            if (!_initialized || _spent) return;
            if (_target == null || _target.IsDead || !_respawn.CanInteract) { Despawn(); return; }
            _remaining -= Time.fixedDeltaTime;
            if (_remaining <= 0f) { Despawn(); return; }
            Vector2 start = transform.position;
            float distance = Mathf.Max(0.1f, _speed) * Time.fixedDeltaTime;
            int count = Physics2D.CircleCast(start, _radius, _direction, _filter, _hits, distance);
            float closest = float.PositiveInfinity;
            PlayerHealth victim = null;
            bool blocked = false;
            for (int i = 0; i < count; i++)
            {
                var hit = _hits[i];
                if (hit.collider == null || hit.distance >= closest) continue;
                PlayerHealth player = hit.collider.GetComponentInParent<PlayerHealth>();
                bool solid = !hit.collider.isTrigger && (_groundLayers.value & (1 << hit.collider.gameObject.layer)) != 0;
                if (player != _target && !solid) continue;
                closest = hit.distance;
                victim = player == _target ? player : null;
                blocked = true;
            }
            if (blocked)
            {
                // Consume before callbacks; invulnerability may reject damage,
                // but the same seed must never wait inside the player's collider.
                Despawn();
                if (victim != null) victim.TakeDamage(_damage);
                return;
            }
            transform.position += (Vector3)(_direction * distance);
        }

        public void Despawn()
        {
            if (_spent) return;
            _spent = true;
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            if (!_initialized) return;
            if (_target != null) _target.Died -= Despawn;
            if (_respawn != null) _respawn.Respawning -= Despawn;
            _initialized = false;
        }
    }
}

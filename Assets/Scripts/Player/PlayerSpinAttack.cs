using System;
using System.Collections.Generic;
using KitchenChaos.Enemy;
using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>Opt-in special. Dash wins, then special, then melee, then ranged.</summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(-45)]
    [RequireComponent(typeof(PlayerAttack), typeof(PlayerRangedAttack), typeof(PlayerMobility))]
    public sealed class PlayerSpinAttack : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _capacity = 100;
        [SerializeField, Min(1)] private int _meleeGain = 20, _rangedGain = 10;
        [SerializeField, Min(1)] private int _damage = 2;
        [SerializeField, Min(0.1f)] private float _radius = 2.8f;
        [SerializeField, Min(0.02f)] private float _windup = .10f, _active = .24f, _recovery = .08f;
        [SerializeField] private LayerMask _targets = 64, _solids = 8;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private AudioClip _swingSound, _impactSound;
        public int Energy { get; private set; }
        public int Capacity => _capacity;
        public bool IsSpinning => isActiveAndEnabled && _spinning;
        public bool IsActive => IsSpinning && Time.time - _started >= _windup && Time.time - _started < _windup + _active;
        public int Facing { get; private set; } = 1;
        public int PoseIndex => !IsSpinning ? 0 : Time.time - _started < _windup ? 0 :
            Time.time - _started >= _windup + _active ? 5 :
            1 + Mathf.Clamp(Mathf.FloorToInt((Time.time - _started - _windup) / _active * 4f), 0, 3);
        public float ActiveProgress => Mathf.Clamp01((Time.time - _started - _windup) / _active);
        public event Action<Vector2> HitConnected;
        private PlayerInputReader _input;
        private PlayerAttack _melee;
        private PlayerRangedAttack _ranged;
        private PlayerMobility _mobility;
        private PlayerHealth _health;
        private PlayerRespawn _respawn;
        private CapsuleCollider2D _capsule;
        private ContactFilter2D _filter, _wallFilter;
        private readonly List<Collider2D> _hits = new();
        private readonly List<RaycastHit2D> _walls = new();
        private readonly HashSet<EnemyHealth> _damaged = new();
        private bool _pressed, _spinning, _ready;
        private float _started;

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>(); _melee = GetComponent<PlayerAttack>();
            _ranged = GetComponent<PlayerRangedAttack>(); _mobility = GetComponent<PlayerMobility>();
            _health = GetComponent<PlayerHealth>(); _respawn = GetComponent<PlayerRespawn>();
            _capsule = GetComponent<CapsuleCollider2D>();
            _filter = new ContactFilter2D { useTriggers = true }; _filter.SetLayerMask(_targets);
            _wallFilter = new ContactFilter2D { useTriggers = false }; _wallFilter.SetLayerMask(_solids);
            _ready = _input != null && _health != null && _respawn != null && _capsule != null;
            if (!_ready) { Debug.LogError("PlayerSpinAttack requires player health, input, capsule and respawn.", this); enabled = false; }
        }
        private void OnEnable()
        {
            if (!_ready) return;
            _melee.HitConnected += GainMelee; _ranged.HitConnected += GainRanged;
            _health.Damaged += Cancel; _health.Died += ResetEnergy; _respawn.Respawning += ResetEnergy;
        }
        private void GainMelee(Vector2 point) => Gain(_meleeGain);
        private void GainRanged(Vector2 point) => Gain(_rangedGain);
        private void Gain(int amount)
        {
            if (!_health.IsDead && !_input.IsGameplayBlocked && _respawn.CanInteract)
                Energy = Mathf.Min(_capacity, Energy + amount);
        }
        private void Update()
        {
            if (_ready && !_input.IsGameplayBlocked && _input.SpecialPressedThisFrame) _pressed = true;
        }
        private void FixedUpdate()
        {
            bool pressed = _pressed; _pressed = false;
            if (!_ready) return;
            if (_health.IsDead || _input.IsGameplayBlocked || !_respawn.CanInteract ||
                _mobility.IsDashing || _mobility.IsHurt || _mobility.IsCrouching)
            { Cancel(); return; }
            if (IsSpinning)
            {
                if (Time.time - _started >= _windup + _active + _recovery) { Cancel(); return; }
                if (IsActive) QueryHits();
                return;
            }
            // No queuing or cancellation of an already committed normal attack.
            if (!pressed || Energy < _capacity || _melee.IsAttacking || _ranged.IsThrowing) return;
            Energy -= _capacity; Facing = _melee.FacingDirection;
            _started = Time.time; _spinning = true; _damaged.Clear();
            if (_audio != null && _swingSound != null) _audio.PlayOneShot(_swingSound, .7f);
        }
        private void QueryHits()
        {
            Vector2 center = _capsule.bounds.center;
            int count = Physics2D.OverlapCircle(center, _radius, _filter, _hits);
            for (int i = 0; i < count && IsActive; i++)
            {
                var c = _hits[i]; if (c == null) continue;
                var enemy = c.GetComponentInParent<EnemyHealth>();
                if (enemy == null || !enemy.isActiveAndEnabled || enemy.CurrentHealth <= 0 || _damaged.Contains(enemy)) continue;
                Vector2 point = c.ClosestPoint(center), delta = point - center;
                // An area attack still cannot damage through a solid wall.
                if (delta.sqrMagnitude > .0001f && Physics2D.Raycast(center, delta.normalized, _wallFilter, _walls, delta.magnitude) > 0) continue;
                _damaged.Add(enemy);
                int before = enemy.CurrentHealth;
                enemy.TakeDamage(_damage, true, center);
                if (enemy == null || enemy.CurrentHealth >= before) continue;
                if (enemy.CurrentHealth > 0)
                {
                    var knockback = enemy.GetComponent<EnemySpinKnockback>();
                    if (knockback != null) knockback.PushAway(center.x);
                }
                HitConnected?.Invoke(point);
                if (_audio != null && _impactSound != null) _audio.PlayOneShot(_impactSound, .35f);
            }
        }
        private void Cancel() { _spinning = false; _pressed = false; _damaged.Clear(); }
        private void ResetEnergy() { Cancel(); Energy = 0; }
        private void OnDisable()
        {
            if (_melee != null) _melee.HitConnected -= GainMelee;
            if (_ranged != null) _ranged.HitConnected -= GainRanged;
            if (_health != null) { _health.Damaged -= Cancel; _health.Died -= ResetEnergy; }
            if (_respawn != null) _respawn.Respawning -= ResetEnergy;
            ResetEnergy();
        }
    }
}

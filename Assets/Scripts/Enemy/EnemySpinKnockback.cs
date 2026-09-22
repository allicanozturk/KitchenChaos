using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>Opt-in ground push for small kinematic enemies; not a universal boss reaction.</summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(25)]
    [RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
    public sealed class EnemySpinKnockback : MonoBehaviour
    {
        [SerializeField] private float _distance = .9f, _duration = .18f;
        [SerializeField] private LayerMask _solids = 8;
        public bool IsPushed => isActiveAndEnabled && _remaining > 0f;
        private Rigidbody2D _body;
        private Collider2D _shape;
        private EnemyHealth _health;
        private float _remaining;
        private int _direction;
        private ContactFilter2D _filter;
        private readonly List<RaycastHit2D> _hits = new();
        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>(); _shape = GetComponent<Collider2D>(); _health = GetComponent<EnemyHealth>();
            _filter = new ContactFilter2D { useTriggers = false }; _filter.SetLayerMask(_solids);
        }
        public void PushAway(float sourceX)
        {
            if (_body == null || _shape == null || _body.bodyType != RigidbodyType2D.Kinematic || _health.CurrentHealth <= 0) return;
            _direction = _body.position.x < sourceX ? -1 : 1; _remaining = _duration;
        }
        private void FixedUpdate()
        {
            if (!IsPushed || _health.CurrentHealth <= 0) return;
            float step = _distance / Mathf.Max(.02f, _duration) * Mathf.Min(_remaining, Time.fixedDeltaTime);
            Bounds bounds = _shape.bounds;
            Vector2 direction = Vector2.right * _direction;
            Vector2 size = new Vector2(Mathf.Max(.05f, bounds.size.x - .04f), Mathf.Max(.05f, bounds.size.y - .08f));
            int count = Physics2D.BoxCast(bounds.center, size, 0f, direction, _filter, _hits, step + .02f);
            for (int i = 0; i < count; i++)
                if (_hits[i].collider != null && Vector2.Dot(_hits[i].normal, direction) < -.5f)
                    step = Mathf.Min(step, Mathf.Max(0, _hits[i].distance - .02f));
            // Kinematic patrols don't fall: don't push them into unsupported space.
            Vector2 foot = new Vector2(bounds.center.x + _direction * (bounds.extents.x + step), bounds.min.y + .1f);
            if (Physics2D.Raycast(foot, Vector2.down, _filter, _hits, .4f) == 0) step = 0;
            _body.MovePosition(_body.position + direction * step);
            _remaining = step <= 0 ? 0 : Mathf.Max(0, _remaining - Time.fixedDeltaTime);
        }
        private void OnEnable() { if (_health != null) _health.Restored += ClearPush; }
        private void ClearPush() { _remaining = 0; }
        private void OnDisable()
        {
            if (_health != null) _health.Restored -= ClearPush;
            ClearPush();
        }
    }
}

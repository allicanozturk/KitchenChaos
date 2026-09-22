using System.Collections.Generic;
using KitchenChaos.Player;
using UnityEngine;
namespace KitchenChaos.Enemy
{
    // Non-homing lob. Only the warned blast damages, never the flying seed.
    public sealed class BellPepperBomb : MonoBehaviour
    {
        private const float Radius = 1.5f, Gravity = 18f, Fuse = .7f;
        private Vector2 _velocity;
        private float _age, _fuse, _burstAge;
        private bool _landed, _burst;
        private LayerMask _solids;
        private SpriteRenderer _visual;
        private Sprite _burstSprite;
        private PlayerHealth _player;
        private Collider2D _playerBody;
        private LineRenderer _warning;
        private ContactFilter2D _filter;
        private readonly List<RaycastHit2D> _hits = new(8);
        public void Initialize(Vector2 start, Vector2 target, Sprite seed, Sprite burst,
            Material material, int sortingLayer, int order, LayerMask solids, PlayerHealth player)
        {
            transform.position = start; _solids = solids; _player = player;
            _playerBody = player.GetComponent<Collider2D>(); _burstSprite = burst;
            _visual = gameObject.AddComponent<SpriteRenderer>();
            _visual.sprite = seed; _visual.sharedMaterial = material;
            _visual.sortingLayerID = sortingLayer; _visual.sortingOrder = order;
            float time = Mathf.Clamp(Mathf.Abs(target.x - start.x) / 7f, .65f, 1.1f);
            _velocity = (target - start) / time + Vector2.up * (.5f * Gravity * time);
            _filter = new ContactFilter2D { useTriggers = false }; _filter.SetLayerMask(solids);
            var ring = new GameObject("BlastWarning"); ring.transform.SetParent(transform, false);
            _warning = ring.AddComponent<LineRenderer>(); _warning.useWorldSpace = true;
            _warning.sharedMaterial = material; _warning.loop = true; _warning.positionCount = 40;
            _warning.startWidth = _warning.endWidth = .065f;
            _warning.sortingLayerID = sortingLayer; _warning.sortingOrder = order - 1;
            _warning.enabled = false;
        }
        private void FixedUpdate()
        {
            if (_visual == null || _burst || _landed) return;
            _age += Time.fixedDeltaTime;
            if (_age > 5f || _player == null || _player.IsDead) { Destroy(gameObject); return; }
            _velocity.y -= Gravity * Time.fixedDeltaTime;
            Vector2 delta = _velocity * Time.fixedDeltaTime;
            int n = Physics2D.CircleCast(transform.position, .32f, delta.normalized, _filter, _hits, delta.magnitude);
            float nearest = delta.magnitude; RaycastHit2D hit = default; bool found = false;
            for (int i = 0; i < n; i++) if (_hits[i].distance <= nearest)
            { nearest = _hits[i].distance; hit = _hits[i]; found = true; }
            transform.position += (Vector3)(delta.normalized * Mathf.Max(0f, nearest - (found ? .01f : 0f)));
            if (!found) return;
            if (hit.normal.y > .5f) { _landed = true; _fuse = Fuse; _warning.enabled = true; }
            else { _velocity.x = 0; _velocity.y = Mathf.Min(0, _velocity.y); }
        }
        private void Update()
        {
            if (_visual == null) return;
            if (_player == null || _player.IsDead) { Destroy(gameObject); return; }
            if (_burst)
            {
                _burstAge += Time.deltaTime;
                _visual.color = new Color(1, 1, 1, 1f - _burstAge / .22f);
                if (_burstAge >= .22f) Destroy(gameObject);
                return;
            }
            if (!_landed) { transform.Rotate(0, 0, 180f * Time.deltaTime); return; }
            transform.rotation = Quaternion.identity;
            _fuse -= Time.deltaTime;
            float pulse = .5f + .5f * Mathf.Sin((Fuse - _fuse) * 40f);
            _visual.color = Color.Lerp(Color.white, new Color(1, .55f, .15f), pulse);
            _warning.startColor = _warning.endColor = new Color(1, .55f, .08f, .65f + .35f * pulse);
            for (int i = 0; i < 40; i++)
            {
                float a = i * Mathf.PI * 2 / 40;
                _warning.SetPosition(i, transform.position + new Vector3(Mathf.Cos(a) * Radius, Mathf.Sin(a) * .14f - .27f, 0));
            }
            if (_fuse <= 0) Explode();
        }
        private void Explode()
        {
            _burst = true; _warning.enabled = false; _visual.sprite = _burstSprite; _visual.color = Color.white;
            if (_playerBody == null || !_playerBody.isActiveAndEnabled || _player.IsDead) return;
            Vector2 center = transform.position;
            Vector2 nearest = _playerBody.ClosestPoint(center);
            if (Vector2.Distance(center, nearest) <= Radius && !Physics2D.Linecast(center, nearest, _solids))
                _player.TakeDamage(1);
        }
    }
}

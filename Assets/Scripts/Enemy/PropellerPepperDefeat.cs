using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>Harmless defeat sprite; its encounter owns and clears it on retry.</summary>
    public sealed class PropellerPepperDefeat : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private LayerMask _solids;
        private float _age, _speed;
        private bool _landed;
        public void Initialize(LayerMask solids) { _solids = solids; _sprite = GetComponent<SpriteRenderer>(); }
        private void Update()
        {
            if (_sprite == null) return;
            _age += Time.deltaTime;
            if (!_landed)
            {
                _speed = Mathf.Min(12f, _speed + 20f * Time.deltaTime);
                float step = _speed * Time.deltaTime;
                var hit = Physics2D.Raycast(transform.position, Vector2.down, step + .05f, _solids);
                if (hit.collider != null)
                { transform.position = new Vector3(transform.position.x, hit.point.y + .025f, transform.position.z); _landed = true; }
                else transform.position += Vector3.down * step;
            }
            _sprite.color = new Color(1f, 1f, 1f, 1f - Mathf.InverseLerp(.9f, 1.4f, _age));
            if (_age >= 1.4f) Destroy(gameObject);
        }
    }
}

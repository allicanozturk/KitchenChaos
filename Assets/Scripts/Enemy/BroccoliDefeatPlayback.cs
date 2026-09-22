using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>Short settling motion and fade; owned by the encounter reset root.</summary>
    public sealed class BroccoliDefeatPlayback : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private Vector3 _rest;
        private float _age;
        private void Awake() { _sprite = GetComponent<SpriteRenderer>(); _rest = transform.position; }
        private void Update()
        {
            _age += Time.deltaTime;
            transform.position = _rest + Vector3.up * (Mathf.Sin(Mathf.Clamp01(_age / .24f) * Mathf.PI) * .16f);
            _sprite.color = new Color(1f, 1f, 1f, 1f - Mathf.InverseLerp(.65f, 1.1f, _age));
            if (_age >= 1.1f) Destroy(gameObject);
        }
    }
}

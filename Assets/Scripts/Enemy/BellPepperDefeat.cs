using UnityEngine;
namespace KitchenChaos.Enemy
{
    public sealed class BellPepperDefeat : MonoBehaviour
    {
        private SpriteRenderer _visual;
        private Sprite[] _poses;
        private float _age;
        public void Initialize(SpriteRenderer visual, Sprite[] poses) { _visual = visual; _poses = poses; }
        private void Update()
        {
            if (_visual == null || _poses == null) { Destroy(gameObject); return; }
            _age += Time.deltaTime;
            _visual.sprite = _poses[_age < .16f ? 11 : _age < .4f ? 12 : 13];
            _visual.color = new Color(1, 1, 1, 1f - Mathf.Clamp01((_age - .9f) / .4f));
            if (_age >= 1.3f) Destroy(gameObject);
        }
    }
}

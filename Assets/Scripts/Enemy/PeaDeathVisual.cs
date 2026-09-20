using UnityEngine;
using UnityEngine.SceneManagement;

namespace KitchenChaos.Enemy
{
    /// <summary>Detached pod split: visual only, with immediate gameplay death.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class PeaDeathVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _splitAnticipation;
        [SerializeField] private Sprite _leftShell;
        [SerializeField] private Sprite _rightShell;
        [SerializeField] private Sprite _pea;
        private EnemyHealth _health;
        private Transform _parent;
        private Vector3 _restPosition;
        private Vector3 _restScale;
        private Quaternion _restRotation;
        private Color _restColor;
        private bool _initialized;
        private bool _spawned;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            if (_spriteRenderer == null || _splitAnticipation == null ||
                _leftShell == null || _rightShell == null || _pea == null)
            {
                Debug.LogWarning("Pea death presentation needs its renderer and four sprites.", this);
                enabled = false;
                return;
            }
            Transform visual = _spriteRenderer.transform;
            _parent = visual.parent;
            _restPosition = visual.localPosition;
            _restRotation = visual.localRotation;
            _restScale = visual.lossyScale;
            _restColor = _spriteRenderer.color;
            _initialized = true;
        }

        private void OnEnable()
        {
            if (_initialized) _health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (_health != null) _health.Died -= OnDied;
        }

        private void OnDied()
        {
            if (_spawned || !_initialized || _parent == null) return;
            _spawned = true;
            var effect = new GameObject(name + " (Pod Split)");
            effect.layer = _spriteRenderer.gameObject.layer;
            // Owned by the level; a restart/unload also removes the detached art.
            SceneManager.MoveGameObjectToScene(effect, gameObject.scene);
            effect.transform.position = _parent.TransformPoint(_restPosition);
            effect.transform.rotation = _parent.rotation * _restRotation;
            effect.transform.localScale = _restScale;
            var body = effect.AddComponent<SpriteRenderer>();
            body.sprite = _splitAnticipation;
            body.sharedMaterial = _spriteRenderer.sharedMaterial;
            body.sortingLayerID = _spriteRenderer.sortingLayerID;
            body.sortingOrder = _spriteRenderer.sortingOrder;
            body.flipX = _spriteRenderer.flipX;
            body.color = _restColor;
            effect.AddComponent<PeaPodPlayback>().Initialize(body, _leftShell, _rightShell, _pea);
        }
    }

    /// <summary>No colliders or damage: peas hop once, roll and fade.</summary>
    internal sealed class PeaPodPlayback : MonoBehaviour
    {
        private const float Anticipation = 0.2f;
        private const float DebrisDuration = 1.05f;
        private SpriteRenderer _body;
        private Sprite[] _art;
        private SpriteRenderer[] _pieces;
        private Vector2[] _velocities;
        private bool[] _bounced;
        private float[] _radii;
        private Vector3 _restScale;
        private Color _color;
        private float _elapsed;
        private float _facing;

        public void Initialize(SpriteRenderer body, Sprite left, Sprite right, Sprite pea)
        {
            _body = body;
            _art = new[] { left, right, pea };
            _restScale = transform.localScale;
            _color = body.color;
            _facing = body.flipX ? -1f : 1f;
        }

        private void Update()
        {
            if (_body == null || Time.deltaTime <= 0f) return;
            _elapsed += Time.deltaTime;
            if (_elapsed >= Anticipation + DebrisDuration)
            {
                Destroy(gameObject);
                return;
            }
            if (_elapsed < Anticipation)
            {
                float t = _elapsed / Anticipation;
                // Feet stay anchored; a small tense pose precedes the shell opening.
                transform.localScale = Vector3.Scale(_restScale,
                    new Vector3(1f + 0.06f * t, 1f - 0.04f * t, 1f));
                return;
            }
            if (_pieces == null) Split();
            float dt = Mathf.Min(Time.deltaTime, 0.05f);
            float fade = 1f - Mathf.Clamp01((_elapsed - Anticipation - 0.6f) / 0.45f);
            for (int i = 0; i < _pieces.Length; i++)
            {
                Vector2 velocity = _velocities[i];
                velocity.y -= 9f * dt;
                Vector3 position = _pieces[i].transform.localPosition;
                position += new Vector3(velocity.x, velocity.y, 0f) * dt;
                // This enemy is stationary on a flat shelf. Purely visual floor
                // is its cached foot baseline, never a gameplay physics body.
                if (position.y < _radii[i])
                {
                    position.y = _radii[i];
                    if (i >= 2 && !_bounced[i])
                    {
                        velocity.y = Mathf.Abs(velocity.y) * 0.35f;
                        _bounced[i] = true;
                    }
                    else { velocity.y = 0f; velocity.x *= Mathf.Max(0f, 1f - 5f * dt); }
                }
                _velocities[i] = velocity;
                _pieces[i].transform.localPosition = position;
                _pieces[i].transform.Rotate(0f, 0f, (i % 2 == 0 ? -1f : 1f) * 190f * dt * _facing);
                Color color = _color; color.a *= fade; _pieces[i].color = color;
            }
        }

        private void Split()
        {
            _body.enabled = false;
            transform.localScale = _restScale;
            _pieces = new SpriteRenderer[5];
            _velocities = new[] { new Vector2(-1.8f, 1.4f), new Vector2(1.7f, 1.8f),
                new Vector2(-2.1f, 2.6f), new Vector2(0.5f, 3f), new Vector2(2.2f, 2.1f) };
            _bounced = new bool[5];
            _radii = new float[5];
            for (int i = 0; i < 5; i++)
            {
                var piece = new GameObject(i < 2 ? "Pod Half" : "Loose Pea");
                piece.layer = gameObject.layer;
                piece.transform.SetParent(transform, false);
                Sprite sprite = _art[i < 2 ? i : 2];
                float width = i < 2 ? 0.65f : i == 2 ? 0.52f : 0.38f;
                piece.transform.localScale = Vector3.one * (width / Mathf.Max(0.01f, sprite.bounds.size.x));
                piece.transform.localPosition = new Vector3(i < 2 ? (i == 0 ? -0.2f : 0.2f) * _facing : 0f,
                    i < 2 ? 1.1f : i == 2 ? 1.8f : 0.85f, 0f);
                _radii[i] = i < 2 ? 0.2f : width * 0.5f;
                _velocities[i].x *= _facing;
                var renderer = piece.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sharedMaterial = _body.sharedMaterial;
                renderer.sortingLayerID = _body.sortingLayerID;
                renderer.sortingOrder = _body.sortingOrder;
                renderer.flipX = _body.flipX;
                renderer.color = _color;
                _pieces[i] = renderer;
            }
        }
    }
}

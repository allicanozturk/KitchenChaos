using UnityEngine;
using UnityEngine.SceneManagement;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Plays a detached cartoon tomato pop without delaying gameplay death.
    /// The temporary objects contain renderers only, never colliders or damage.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class TomatoDeathVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _surprisedSprite;
        [Tooltip("Two red wedges, one green leaf, then one seed.")]
        [SerializeField] private Sprite[] _fragmentSprites;
        [SerializeField, Min(0.05f)] private float _anticipationDuration = 0.24f;
        [SerializeField, Min(0.1f)] private float _debrisDuration = 0.7f;

        private EnemyHealth _health;
        private Transform _visualParent;
        private Vector3 _restLocalPosition;
        private Quaternion _restLocalRotation;
        private Vector3 _restWorldScale;
        private Color _restColor;
        private bool _initialized;
        private bool _spawned;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            bool valid = _spriteRenderer != null &&
                         _spriteRenderer.transform != transform &&
                         _spriteRenderer.transform.IsChildOf(transform) &&
                         _surprisedSprite != null &&
                         _fragmentSprites != null && _fragmentSprites.Length >= 4;
            if (valid)
            {
                for (int i = 0; i < 4; i++)
                    valid &= _fragmentSprites[i] != null;
            }

            if (!valid)
            {
                Debug.LogWarning($"{nameof(TomatoDeathVisual)} needs a visual child renderer, surprised sprite and four fragment sprites. Death gameplay is unaffected.", this);
                enabled = false;
                return;
            }

            // Cache before hit tint/squash. The root and Visual have compensating
            // nonuniform scales; copying only Visual.localScale would distort it.
            Transform visual = _spriteRenderer.transform;
            _visualParent = visual.parent;
            _restLocalPosition = visual.localPosition;
            _restLocalRotation = visual.localRotation;
            _restWorldScale = visual.lossyScale;
            _restColor = _spriteRenderer.color;
            _initialized = true;
        }

        private void OnEnable()
        {
            if (_initialized)
                _health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.Died -= OnDied;
        }

        private void OnDied()
        {
            if (_spawned || !_initialized || _spriteRenderer == null || _visualParent == null)
                return;

            _spawned = true;
            GameObject effect = new GameObject($"{name} (Tomato Pop)");
            effect.layer = _spriteRenderer.gameObject.layer;
            // Scene unload/restart owns cleanup, including during pause.
            SceneManager.MoveGameObjectToScene(effect, gameObject.scene);
            effect.transform.position = _visualParent.TransformPoint(_restLocalPosition);
            effect.transform.rotation = _visualParent.rotation * _restLocalRotation;
            effect.transform.localScale = _restWorldScale;

            SpriteRenderer renderer = effect.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = _spriteRenderer.sharedMaterial;
            renderer.sortingLayerID = _spriteRenderer.sortingLayerID;
            renderer.sortingOrder = _spriteRenderer.sortingOrder;
            renderer.flipX = _spriteRenderer.flipX;
            renderer.flipY = _spriteRenderer.flipY;
            renderer.color = _restColor;
            renderer.sprite = _surprisedSprite;

            effect.AddComponent<TomatoPopPlayback>().Initialize(renderer, _fragmentSprites,
                _anticipationDuration, _debrisDuration);
        }
    }

    // Runtime-only helper. The enemy itself still deactivates immediately.
    internal sealed class TomatoPopPlayback : MonoBehaviour
    {
        private const int FragmentCount = 12;
        private const float Gravity = 8f;

        // Six wedges, two leaves, four seeds; deterministic without consuming
        // UnityEngine.Random or changing any gameplay random sequence.
        private static readonly Vector2[] Velocities =
        {
            new Vector2(-1.35f, 2.2f), new Vector2(1.3f, 2.3f),
            new Vector2(-0.85f, 2.8f), new Vector2(0.8f, 2.95f),
            new Vector2(-0.45f, 1.9f), new Vector2(0.4f, 2.05f),
            new Vector2(-0.65f, 3.15f), new Vector2(0.75f, 3.35f),
            new Vector2(-1.55f, 2.65f), new Vector2(1.5f, 2.8f),
            new Vector2(-0.25f, 3.4f), new Vector2(0.25f, 3.1f)
        };

        private SpriteRenderer _body;
        private Sprite[] _sprites;
        private SpriteRenderer[] _fragments;
        private Vector3 _restScale;
        private Vector3 _burstOrigin;
        private Color _restColor;
        private float _anticipationDuration;
        private float _debrisDuration;
        private float _elapsed;
        private float _facing;

        public void Initialize(SpriteRenderer body, Sprite[] sprites, float anticipation, float debris)
        {
            _body = body;
            _sprites = sprites;
            _restScale = transform.localScale;
            _restColor = body.color;
            _facing = body.flipX ? -1f : 1f;
            // Keep accidental Inspector values from producing lingering debris.
            _anticipationDuration = Mathf.Clamp(anticipation, 0.05f, 0.6f);
            _debrisDuration = Mathf.Clamp(debris, 0.1f, 1.5f);
            Bounds bounds = body.sprite.bounds;
            _burstOrigin = new Vector3(0f, bounds.min.y + bounds.size.y * 0.46f, 0f);
        }

        private void Update()
        {
            // Scaled time pauses anticipation, debris, fade and cleanup together.
            if (_body == null || Time.deltaTime <= 0f)
                return;

            _elapsed += Time.deltaTime;
            if (_elapsed >= _anticipationDuration + _debrisDuration)
            {
                Destroy(gameObject);
                return;
            }

            if (_elapsed < _anticipationDuration)
            {
                float progress = _elapsed / _anticipationDuration;
                float swell = 1f + 0.15f * Mathf.SmoothStep(0f, 1f, progress);
                // The surprised art shares the idle ground pivot: feet stay put.
                transform.localScale = _restScale * swell;
                return;
            }

            if (_fragments == null)
                Pop();

            float time = _elapsed - _anticipationDuration;
            float progressAfterPop = time / _debrisDuration;
            Color color = _restColor;
            color.a *= 1f - Mathf.Clamp01((progressAfterPop - 0.35f) / 0.65f);
            for (int i = 0; i < FragmentCount; i++)
            {
                SpriteRenderer fragment = _fragments[i];
                Vector2 velocity = Velocities[i];
                fragment.transform.localPosition = _burstOrigin + new Vector3(
                    velocity.x * _facing * time,
                    velocity.y * time - 0.5f * Gravity * time * time, 0f);
                float spin = (i % 2 == 0 ? -1f : 1f) * (180f + i * 23f) * _facing;
                fragment.transform.localRotation = Quaternion.Euler(0f, 0f, i * 29f + spin * time);
                fragment.color = color;
            }
        }

        private void Pop()
        {
            // Hide the entire tomato permanently; no intact falling body remains.
            _body.enabled = false;
            transform.localScale = _restScale;
            _fragments = new SpriteRenderer[FragmentCount];
            for (int i = 0; i < FragmentCount; i++)
            {
                int spriteIndex = i < 6 ? i % 2 : i < 8 ? 2 : 3;
                Sprite sprite = _sprites[spriteIndex];
                GameObject piece = new GameObject(i < 6 ? "Tomato Wedge" : i < 8 ? "Leaf" : "Seed");
                piece.layer = gameObject.layer;
                piece.transform.SetParent(transform, false);
                // Fragment artwork is tightly cropped; normalize width so the
                // pop stays modest regardless of each fragment texture's PPU.
                float width = i < 6 ? 0.5f + 0.07f * (i % 4) : i < 8 ? 0.32f : 0.09f;
                float scale = width / Mathf.Max(0.01f, sprite.bounds.size.x);
                piece.transform.localScale = Vector3.one * scale;

                SpriteRenderer renderer = piece.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sharedMaterial = _body.sharedMaterial;
                renderer.sortingLayerID = _body.sortingLayerID;
                renderer.sortingOrder = _body.sortingOrder;
                renderer.flipX = _body.flipX;
                renderer.flipY = _body.flipY;
                renderer.color = _restColor;
                _fragments[i] = renderer;
            }
        }
    }
}

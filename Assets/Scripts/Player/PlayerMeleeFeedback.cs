using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Temporary, code-animated spatula and hit sparks. Reads the attack clock;
    /// never moves the physics root or decides whether a hit deals damage.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerAttack), typeof(PlayerHealth), typeof(PlayerRespawn))]
    [DefaultExecutionOrder(80)]
    public sealed class PlayerMeleeFeedback : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _playerRenderer;
        [SerializeField] private ChefLocomotionVisual _locomotionVisual;
        [SerializeField] private Transform _weaponPivot;
        [SerializeField] private SpriteRenderer[] _weaponParts;
        [SerializeField] private SpriteRenderer[] _swingArc;
        [SerializeField] private SpriteRenderer[] _spinTrailOuter;
        [SerializeField] private SpriteRenderer[] _spinTrailCore;
        [SerializeField] private Transform[] _impactRoots;
        [SerializeField] private float _raisedAngle = 80f;
        [SerializeField] private bool _hideWeaponOnDeath;

        private const float IdleAngle = -40f;
        private const float FollowThroughAngle = -55f;
        private const float ImpactDuration = 0.14f;

        private PlayerAttack _attack;
        private PlayerHealth _health;
        private PlayerRespawn _respawn;
        private PlayerRangedAttack _ranged;
        private PlayerSpinAttack _spin;
        private static readonly float[] SpinWeaponAngles = { 150f, 165f, 180f, 180f, 0f, 25f };
        private Vector3 _rightGrip;
        private Color[] _weaponColors;
        private bool[] _weaponVisibility;
        private Vector3[] _arcPositions;
        private float[] _arcAngles;
        private Color[] _arcColors;
        private Impact[] _impacts;
        private int _nextImpact;
        private bool _initialized;
        private float _spinTrailFade;
        private float _spinTrailProgress;

        private sealed class Impact
        {
            public Transform Root;
            public SpriteRenderer[] Rays;
            public Color[] Colors;
            public Vector3 Position;
            public Vector3 Scale;
            public float Remaining;
            public float Duration;
            public float Strength;
        }

        private void Awake()
        {
            _attack = GetComponent<PlayerAttack>();
            _health = GetComponent<PlayerHealth>();
            _respawn = GetComponent<PlayerRespawn>();
            _ranged = GetComponent<PlayerRangedAttack>();
            _spin = GetComponent<PlayerSpinAttack>();
            if (_playerRenderer == null || _weaponPivot == null ||
                !_weaponPivot.IsChildOf(transform) || _weaponPivot == transform ||
                _weaponParts == null || _swingArc == null || _impactRoots == null)
            {
                Debug.LogError($"{nameof(PlayerMeleeFeedback)} needs the prototype weapon and effect references.", this);
                enabled = false;
                return;
            }

            _rightGrip = _weaponPivot.localPosition;
            _rightGrip.x = Mathf.Abs(_rightGrip.x);
            _weaponColors = new Color[_weaponParts.Length];
            _weaponVisibility = new bool[_weaponParts.Length];
            for (int i = 0; i < _weaponParts.Length; i++)
            {
                _weaponColors[i] = _weaponParts[i].color;
                _weaponVisibility[i] = _weaponParts[i].enabled;
            }

            _arcPositions = new Vector3[_swingArc.Length];
            _arcAngles = new float[_swingArc.Length];
            _arcColors = new Color[_swingArc.Length];
            for (int i = 0; i < _swingArc.Length; i++)
            {
                _arcPositions[i] = _swingArc[i].transform.localPosition;
                _arcAngles[i] = _swingArc[i].transform.localEulerAngles.z;
                _arcColors[i] = _swingArc[i].color;
                _swingArc[i].enabled = false;
            }

            _impacts = new Impact[_impactRoots.Length];
            for (int i = 0; i < _impacts.Length; i++)
            {
                var impact = new Impact
                {
                    Root = _impactRoots[i],
                    Rays = _impactRoots[i].GetComponentsInChildren<SpriteRenderer>(true),
                    Scale = _impactRoots[i].localScale
                };
                impact.Colors = new Color[impact.Rays.Length];
                for (int j = 0; j < impact.Rays.Length; j++)
                {
                    impact.Colors[j] = impact.Rays[j].color;
                    impact.Rays[j].enabled = false;
                }
                _impacts[i] = impact;
            }
            _initialized = true;
        }

        private void OnEnable()
        {
            if (!_initialized)
                return;
            _attack.HitConnected += OnHitConnected;
            if (_spin != null) _spin.HitConnected += OnSpinHitConnected;
            _health.Died += ClearEffects;
            _respawn.Respawned += ClearEffects;
            ClearEffects();
        }

        private void LateUpdate()
        {
            UpdateWeaponVisibility();
            UpdateSpinTrail();
            float angle = IdleAngle;
            float progress = _attack.PhaseProgress;
            if (!_health.IsDead && _attack.isActiveAndEnabled)
            {
                switch (_attack.Phase)
                {
                    case PlayerAttack.AttackPhase.Windup:
                        angle = Mathf.Lerp(IdleAngle, _raisedAngle, Mathf.SmoothStep(0f, 1f, progress));
                        break;
                    case PlayerAttack.AttackPhase.Active:
                        angle = Mathf.Lerp(_raisedAngle, FollowThroughAngle, progress);
                        break;
                    case PlayerAttack.AttackPhase.Recovery:
                        angle = Mathf.Lerp(FollowThroughAngle, IdleAngle, Mathf.SmoothStep(0f, 1f, progress));
                        break;
                }
            }

            // A painted attack frame already owns the arm pose. Keep its spatula
            // angle in the same pose instead of running a second sweep in that fist.
            // Apply only here: ClearEffects must still force idle on death/disable.
            if (_locomotionVisual != null &&
                _locomotionVisual.TryGetAttackWeaponAngle(out float poseAngle))
                angle = poseAngle;

            int direction = _attack.FacingDirection;
            if (_spin != null && _spin.IsSpinning)
            {
                // Same separate spatula stays attached to each drawn fist.
                float[] angles = SpinWeaponAngles;
                angle = angles[Mathf.Clamp(_spin.PoseIndex, 0, angles.Length - 1)];
            }
            SetWeaponPose(angle, direction);
            // Runs after PlayerDamageFeedback, so the spatula shares hurt/death
            // tint and transparency instead of floating beside an invisible body.
            for (int i = 0; i < _weaponParts.Length; i++)
                _weaponParts[i].color = _weaponColors[i] * _playerRenderer.color;

            bool showArc = !_health.IsDead && _attack.isActiveAndEnabled &&
                _attack.Phase == PlayerAttack.AttackPhase.Active;
            for (int i = 0; i < _swingArc.Length; i++)
            {
                SpriteRenderer segment = _swingArc[i];
                segment.enabled = showArc;
                if (!showArc)
                    continue;

                Vector3 position = _arcPositions[i];
                if (_attack.IsCrouchedSwing)
                {
                    // Lower the existing arc with the low hand/hitbox, in world units.
                    position.y -= 0.4f / Mathf.Max(0.001f, Mathf.Abs(segment.transform.parent.lossyScale.y));
                }
                position.x *= direction;
                segment.transform.localPosition = position;
                segment.transform.localRotation = Quaternion.Euler(0f, 0f,
                    direction > 0 ? _arcAngles[i] : 180f - _arcAngles[i]);
                Color color = _arcColors[i];
                color.a *= (1f - 0.65f * progress) * _playerRenderer.color.a;
                segment.color = color;
            }

            foreach (Impact impact in _impacts)
            {
                if (impact.Remaining <= 0f)
                    continue;

                float fraction = Mathf.Clamp01(1f - impact.Remaining / impact.Duration);
                // Although pooled under the player, an impact stays at its world
                // contact point; running or teleporting does not drag it along.
                impact.Root.position = impact.Position;
                impact.Root.localScale = impact.Scale * impact.Strength * (0.65f + 0.8f * fraction);
                impact.Remaining = Mathf.Max(0f, impact.Remaining - Time.deltaTime);
                for (int i = 0; i < impact.Rays.Length; i++)
                {
                    Color color = impact.Colors[i];
                    color.a *= 1f - fraction;
                    impact.Rays[i].color = color;
                    impact.Rays[i].enabled = impact.Remaining > 0f;
                }
            }
        }

        private void SetWeaponPose(float angle, int direction)
        {
            Vector3 grip = _rightGrip;
            grip.x *= direction;
            // Optional presentation-only attachment. The chef supplies the current
            // frame's fist after sprite mirroring/breathing, so do not mirror twice.
            // Older prefabs without locomotion visuals keep their authored grip.
            if (_locomotionVisual != null &&
                _locomotionVisual.TryGetGripWorldPosition(out Vector3 handPosition))
            {
                Vector3 animatedGrip = _weaponPivot.parent.InverseTransformPoint(handPosition);
                grip.x = animatedGrip.x;
                grip.y = animatedGrip.y;
            }
            _weaponPivot.localPosition = grip;
            _weaponPivot.localRotation = Quaternion.Euler(0f, 0f, direction > 0 ? angle : 180f - angle);
        }

        private void UpdateSpinTrail()
        {
            if (_spin == null || !_spin.IsSpinning || _health.IsDead)
            {
                _spinTrailFade = 0f;
                HideSpinTrail();
                return;
            }
            if (_spin.IsActive)
            {
                _spinTrailProgress = _spin.ActiveProgress;
                _spinTrailFade = .08f;
            }
            else _spinTrailFade = Mathf.Max(0f, _spinTrailFade - Time.deltaTime);
            if (_spinTrailFade <= 0f) { HideSpinTrail(); return; }
            // Pure presentation: a tapered elliptical ribbon around the waist.
            // World-space dimensions avoid the player's non-uniform root scale.
            Vector3 center = transform.position + Vector3.down * .15f;
            DrawSpinRibbon(_spinTrailOuter, center, false);
            DrawSpinRibbon(_spinTrailCore, center, true);
        }

        private void DrawSpinRibbon(SpriteRenderer[] segments, Vector3 center, bool core)
        {
            if (segments == null || segments.Length == 0) return;
            float sweep = 250f * Mathf.Deg2Rad;
            float head = (25f + 360f * _spinTrailProgress) * Mathf.Deg2Rad;
            for (int i = 0; i < segments.Length; i++)
            {
                var sr = segments[i]; if (sr == null || sr.sprite == null) continue;
                float t = (float)i / segments.Length;
                float a = head - sweep * (1f - t), b = a + sweep / segments.Length;
                Vector3 p = new Vector3(Mathf.Cos(a) * 2.55f * _spin.Facing, Mathf.Sin(a) * .85f, 0f);
                Vector3 q = new Vector3(Mathf.Cos(b) * 2.55f * _spin.Facing, Mathf.Sin(b) * .85f, 0f);
                Vector3 delta = q - p;
                sr.transform.SetPositionAndRotation(center + (p + q) * .5f,
                    Quaternion.Euler(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg));
                Vector3 parentScale = sr.transform.parent.lossyScale;
                float width = Mathf.Lerp(.035f, core ? .11f : .30f, Mathf.Pow(t, .65f));
                sr.transform.localScale = new Vector3((delta.magnitude + .025f) / sr.sprite.bounds.size.x / Mathf.Max(.001f, Mathf.Abs(parentScale.x)),
                    width / sr.sprite.bounds.size.y / Mathf.Max(.001f, Mathf.Abs(parentScale.y)), 1f);
                // Upper half is behind the chef, lower half passes in front.
                bool behind = p.y + q.y > 0f;
                sr.sortingLayerID = _playerRenderer.sortingLayerID;
                sr.sortingOrder = _playerRenderer.sortingOrder + (behind ? -2 : 5) + (core ? 1 : 0);
                Color color = core ? new Color(1f, .98f, .68f) : new Color(1f, .67f, .06f);
                color.a = Mathf.Lerp(.08f, 1f, t) * (_spinTrailFade / .08f) * _playerRenderer.color.a * (behind ? .65f : 1f);
                sr.color = color; sr.enabled = true;
            }
        }

        private void HideSpinTrail()
        {
            if (_spinTrailOuter != null) foreach (var sr in _spinTrailOuter) if (sr != null) sr.enabled = false;
            if (_spinTrailCore != null) foreach (var sr in _spinTrailCore) if (sr != null) sr.enabled = false;
        }

        private void OnHitConnected(Vector2 point) => SpawnImpact(point, 1f, ImpactDuration);
        private void OnSpinHitConnected(Vector2 point) => SpawnImpact(point, 1.65f, .20f);

        private void SpawnImpact(Vector2 point, float strength, float duration)
        {
            if (_impacts.Length == 0 || _health.IsDead)
                return;

            Impact impact = _impacts[_nextImpact];
            _nextImpact = (_nextImpact + 1) % _impacts.Length;
            impact.Position = new Vector3(point.x, point.y, transform.position.z);
            impact.Remaining = duration;
            impact.Duration = duration;
            impact.Strength = strength;
        }

        private void ClearEffects()
        {
            _spinTrailFade = 0f;
            HideSpinTrail();
            _nextImpact = 0;
            foreach (SpriteRenderer segment in _swingArc)
                segment.enabled = false;
            foreach (Impact impact in _impacts)
            {
                impact.Remaining = 0f;
                impact.Root.localScale = impact.Scale;
                foreach (SpriteRenderer ray in impact.Rays)
                    ray.enabled = false;
            }
            SetWeaponPose(IdleAngle, _attack.FacingDirection);
            UpdateWeaponVisibility();
        }

        private void UpdateWeaponVisibility()
        {
            for (int i = 0; i < _weaponParts.Length; i++)
                if (_weaponParts[i] != null)
                    _weaponParts[i].enabled = _weaponVisibility[i] &&
                        !(_hideWeaponOnDeath && _health.IsDead) && !(_ranged != null && _ranged.IsThrowing);
        }

        private void OnDisable()
        {
            if (!_initialized)
                return;
            if (_attack != null)
                _attack.HitConnected -= OnHitConnected;
            if (_spin != null) _spin.HitConnected -= OnSpinHitConnected;
            if (_health != null)
                _health.Died -= ClearEffects;
            if (_respawn != null)
                _respawn.Respawned -= ClearEffects;
            ClearEffects();
            for (int i = 0; i < _weaponParts.Length; i++)
                if (_weaponParts[i] != null)
                {
                    _weaponParts[i].color = _weaponColors[i];
                    if (_hideWeaponOnDeath)
                        _weaponParts[i].enabled = _weaponVisibility[i];
                }
        }
    }
}

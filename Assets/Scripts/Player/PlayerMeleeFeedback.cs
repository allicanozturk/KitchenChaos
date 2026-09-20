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
        [SerializeField] private Transform[] _impactRoots;
        [SerializeField] private float _raisedAngle = 80f;
        [SerializeField] private bool _hideWeaponOnDeath;

        private const float IdleAngle = -40f;
        private const float FollowThroughAngle = -55f;
        private const float ImpactDuration = 0.14f;

        private PlayerAttack _attack;
        private PlayerHealth _health;
        private PlayerRespawn _respawn;
        private Vector3 _rightGrip;
        private Color[] _weaponColors;
        private bool[] _weaponVisibility;
        private Vector3[] _arcPositions;
        private float[] _arcAngles;
        private Color[] _arcColors;
        private Impact[] _impacts;
        private int _nextImpact;
        private bool _initialized;

        private sealed class Impact
        {
            public Transform Root;
            public SpriteRenderer[] Rays;
            public Color[] Colors;
            public Vector3 Position;
            public Vector3 Scale;
            public float Remaining;
        }

        private void Awake()
        {
            _attack = GetComponent<PlayerAttack>();
            _health = GetComponent<PlayerHealth>();
            _respawn = GetComponent<PlayerRespawn>();
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
            _health.Died += ClearEffects;
            _respawn.Respawned += ClearEffects;
            ClearEffects();
        }

        private void LateUpdate()
        {
            UpdateWeaponVisibility();
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

                float fraction = Mathf.Clamp01(1f - impact.Remaining / ImpactDuration);
                // Although pooled under the player, an impact stays at its world
                // contact point; running or teleporting does not drag it along.
                impact.Root.position = impact.Position;
                impact.Root.localScale = impact.Scale * (0.65f + 0.8f * fraction);
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

        private void OnHitConnected(Vector2 point)
        {
            if (_impacts.Length == 0 || _health.IsDead)
                return;

            Impact impact = _impacts[_nextImpact];
            _nextImpact = (_nextImpact + 1) % _impacts.Length;
            impact.Position = new Vector3(point.x, point.y, transform.position.z);
            impact.Remaining = ImpactDuration;
        }

        private void ClearEffects()
        {
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
            if (!_hideWeaponOnDeath)
                return;

            for (int i = 0; i < _weaponParts.Length; i++)
                if (_weaponParts[i] != null)
                    _weaponParts[i].enabled = _weaponVisibility[i] && !_health.IsDead;
        }

        private void OnDisable()
        {
            if (!_initialized)
                return;
            if (_attack != null)
                _attack.HitConnected -= OnHitConnected;
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

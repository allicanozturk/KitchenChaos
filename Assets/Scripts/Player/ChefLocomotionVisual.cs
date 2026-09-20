using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Opt-in Chef A locomotion/action presentation. Reads gameplay state without moving
    /// the physics root, changing facing, or taking ownership of damage tint.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(40)]
    public sealed class ChefLocomotionVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite[] _runFrames = new Sprite[RunFrameCount];
        [SerializeField] private Vector2[] _runGripPoints = new Vector2[RunFrameCount];
        [SerializeField] private Vector2 _idleGripPoint = new Vector2(0.3525671f, 1.37715398f);
        // Optional packs: Rise/Apex/Fall and Windup/Active/Recovery respectively.
        [SerializeField] private Sprite[] _jumpFrames = new Sprite[ActionFrameCount];
        [SerializeField] private Vector2[] _jumpGripPoints = new Vector2[ActionFrameCount];
        [SerializeField] private Sprite[] _attackFrames = new Sprite[ActionFrameCount];
        [SerializeField] private Vector2[] _attackGripPoints = new Vector2[ActionFrameCount];
        [SerializeField] private Sprite[] _deathFrames = new Sprite[DeathFrameCount];
        [SerializeField] private Sprite[] _crouchDeathFrames = new Sprite[DeathFrameCount];
        private Sprite[] _activeDeathFrames;
        // Opt-in movement test art: upright hurt, crouch, ground dash, crouched hurt.
        [SerializeField] private Sprite[] _mobilityFrames = new Sprite[4];
        [SerializeField] private Vector2[] _mobilityGripPoints = new Vector2[4];
        [SerializeField] private Sprite[] _crouchWalkFrames = new Sprite[4];
        [SerializeField] private Vector2[] _crouchWalkGripPoints = new Vector2[4];
        [SerializeField] private Sprite[] _hurtFrames = new Sprite[3];
        [SerializeField] private Vector2[] _hurtGripPoints = new Vector2[3];
        [SerializeField] private Sprite[] _crouchHurtFrames = new Sprite[3];
        [SerializeField] private Vector2[] _crouchHurtGripPoints = new Vector2[3];
        private float _crouchWalkClock;
        private Vector3 _presentationOffset;
        private PlayerMobility _mobility;

        private const int RunFrameCount = 6;
        private const int ActionFrameCount = 3;
        private const int DeathFrameCount = 6;
        private const float DeathPosesEnd = 0.7f;
        private const float DeathFloorTolerance = 0.08f;
        private const float ApexSpeed = 1.2f;
        private const float RunFramesPerSecond = 10f;
        private const float ReferenceMoveSpeed = 5f;
        private const float MovementThreshold = 0.1f;
        private const float InputThreshold = 0.01f;
        private const float BreathDuration = 2.2f;
        private const float BreathWidth = 0.008f;
        private const float BreathHeight = 0.016f;
        private const float ScaleResponseTime = 0.05f;

        private PlayerInputReader _input;
        private Rigidbody2D _rigidbody;
        private PlayerJump _jump;
        private PlayerAttack _attack;
        private PlayerHealth _health;
        private PlayerRespawn _respawn;
        private Transform _visualTransform;
        private Vector3 _restScale;
        private Sprite _restSprite;
        private Vector2 _currentGripPoint;
        private float _runFrameClock;
        private float _breathClock;
        private bool _running;
        private bool _jumpLatched;
        private bool _acceptedJumpFlight;
        private bool _hasGroundSample;
        private bool _hasJumpPack;
        private bool _hasAttackPack;
        private bool _hasDeathPack;
        private bool _deathPrepared;
        private bool _groundDeath;
        private Collider2D _lastLivingGround;
        private BoxCollider2D _deathGround;
        private Bounds _deathSupportBounds;
        private Vector3 _deathOriginLocalPosition;
        private float _deathFallStartProgress;
        private PlayerAttack.AttackPhase _displayedAttackPhase;
        private float _attackWeaponAngle;
        private bool _initialized;

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _jump = GetComponent<PlayerJump>();
            _attack = GetComponent<PlayerAttack>();
            _health = GetComponent<PlayerHealth>();
            _respawn = GetComponent<PlayerRespawn>();
            _mobility = GetComponent<PlayerMobility>();

            if (!HasRequiredReferences())
            {
                Debug.LogError(
                    $"{nameof(ChefLocomotionVisual)} needs a child SpriteRenderer, idle sprite, " +
                    "six run sprites and matching finite grip points, plus the existing player components.",
                    this);
                enabled = false;
                return;
            }

            _visualTransform = _spriteRenderer.transform;
            _restScale = _visualTransform.localScale;
            _restSprite = _spriteRenderer.sprite;
            _currentGripPoint = _idleGripPoint;
            _hasJumpPack = HasActionPack(_jumpFrames, _jumpGripPoints);
            _hasAttackPack = HasActionPack(_attackFrames, _attackGripPoints);
            _hasDeathPack = HasDeathPack();
            _initialized = true;
        }

        private void OnEnable()
        {
            if (!_initialized)
                return;

            _jump.Jumped += OnJumped;
            _health.Died += OnDied;
            _respawn.Respawned += ResetTransientState;
            ResetTransientState();
        }

        private void FixedUpdate()
        {
            // Order 40 follows PlayerJump's ground query. Respawn deliberately
            // clears ground state, so wait for this sample before showing an air pose.
            _hasGroundSample = _initialized && _jump.isActiveAndEnabled &&
                !_health.IsDead && !_input.IsGameplayBlocked && _rigidbody.simulated;
            // SuspendForDeath clears PlayerJump before Died is raised. Preserve the
            // last living contact, then verify its geometry at the actual death pose.
            if (_hasGroundSample)
                _lastLivingGround = _jump.IsGrounded && !_jumpLatched &&
                    Mathf.Abs(_rigidbody.linearVelocity.y) <= ApexSpeed ? _jump.GroundCollider : null;
        }

        private void LateUpdate()
        {
            if (!_initialized || Time.deltaTime <= 0f)
                return;

            ClearPresentationOffset();

            if (_health.IsDead && _hasDeathPack)
            {
                if (!_deathPrepared)
                    OnDied();
                ApplyDeathPose();
                return;
            }

            // Health can be disabled to cancel death without sending Respawned.
            if (_deathPrepared)
                ResetTransientState();

            // Ground overlap briefly remains true after an accepted takeoff.
            // Once airborne (or no longer rising), normal ground state wins.
            if (_jumpLatched && (!_jump.IsGrounded || _rigidbody.linearVelocity.y <= 0f))
                _jumpLatched = false;

            if (_health.IsDead || _input.IsGameplayBlocked || !_rigidbody.simulated)
            {
                _hasGroundSample = false;
                ApplyNeutralPose();
                return;
            }

            bool airborne = _jumpLatched || !_jump.IsGrounded;
            // Track landing even when an attack currently hides the jump animation.
            if (_hasGroundSample && !airborne)
                _acceptedJumpFlight = false;

            if (_mobility != null && _mobility.isActiveAndEnabled)
            {
                bool showHurt = _mobility.IsHurtVisual && !_mobility.IsDashing && !_attack.IsAttacking;
                if (showHurt && ApplyHurtSequence())
                {
                    _crouchWalkClock = 0f;
                    return;
                }
                bool crouchWalking = _mobility.IsCrouching && !_mobility.IsHurt &&
                    !_mobility.IsDashing && !airborne &&
                    Mathf.Abs(_input.Horizontal) >= InputThreshold &&
                    Mathf.Abs(_rigidbody.linearVelocity.x) > MovementThreshold;
                if (crouchWalking && HasCrouchWalkPack())
                {
                    // A full stride covers 1.35 world units. More actual in-between
                    // drawings, not faster repetition of the old two near-identical poses.
                    _crouchWalkClock = Mathf.Repeat(_crouchWalkClock + Time.deltaTime *
                        _crouchWalkFrames.Length * Mathf.Abs(_rigidbody.linearVelocity.x) / 1.35f,
                        _crouchWalkFrames.Length);
                    ApplyActionPose(_crouchWalkFrames, _crouchWalkGripPoints,
                        Mathf.Clamp(Mathf.FloorToInt(_crouchWalkClock), 0, _crouchWalkFrames.Length - 1), true);
                    return;
                }
                _crouchWalkClock = 0f;
                int pose = _mobility.IsHurt ? (_mobility.IsCrouching ? 3 : 0) :
                    _mobility.IsDashing ? 2 : _mobility.IsCrouching ? 1 : -1;
                if (pose >= 0 && _mobilityFrames != null && _mobilityFrames.Length == 4 &&
                    _mobilityGripPoints != null && _mobilityGripPoints.Length == 4 && _mobilityFrames[pose] != null)
                {
                    ApplyActionPose(_mobilityFrames, _mobilityGripPoints, pose, true);
                    if (_mobility.IsHurt)
                    {
                        float flinch = Mathf.Sin(_mobility.HurtProgress * Mathf.PI);
                        _visualTransform.localScale = Vector3.Scale(_restScale,
                            new Vector3(1f + 0.035f * flinch, 1f - 0.035f * flinch, 1f));
                    }
                    return;
                }
            }

            if (_attack.isActiveAndEnabled && _attack.IsAttacking)
            {
                int frame = _attack.Phase == PlayerAttack.AttackPhase.Windup ? 0 :
                    _attack.Phase == PlayerAttack.AttackPhase.Active ? 1 : 2;
                ApplyActionPose(_attackFrames, _attackGripPoints, frame, _hasAttackPack);
                if (_hasAttackPack)
                {
                    // Sprite, fist and wrist orientation are one authored pose.
                    // A second procedural sweep in a held fist reads as an uppercut.
                    _attackWeaponAngle = frame == 0 ? 135f : frame == 1 ? -35f : -40f;
                    _displayedAttackPhase = _attack.Phase;
                }
                return;
            }

            if (!_jump.isActiveAndEnabled || !_hasGroundSample)
            {
                ApplyNeutralPose();
                return;
            }

            if (airborne)
            {
                float verticalSpeed = _rigidbody.linearVelocity.y;
                // Walking off an edge starts in Fall, not an unearned apex pose.
                int frame = verticalSpeed > ApexSpeed ? 0 :
                    _acceptedJumpFlight && verticalSpeed >= -ApexSpeed ? 1 : 2;
                ApplyActionPose(_jumpFrames, _jumpGripPoints, frame, _hasJumpPack);
                return;
            }

            float speed = Mathf.Abs(_rigidbody.linearVelocity.x);
            bool hasMovementIntent = Mathf.Abs(_input.Horizontal) >= InputThreshold;
            if (speed > MovementThreshold && hasMovementIntent)
                ApplyRunPose(speed);
            else
                ApplyIdlePose();
        }

        /// <summary>
        /// Active frame's fist position, including visual breathing and facing.
        /// SpriteRenderer flips do not transform child objects, so mirror once here.
        /// The weapon presenter converts this world point into its parent's space.
        /// </summary>
        public bool TryGetGripWorldPosition(out Vector3 position)
        {
            position = Vector3.zero;
            if (!_initialized || !isActiveAndEnabled || _spriteRenderer == null || _visualTransform == null)
                return false;

            Vector3 point = new Vector3(_currentGripPoint.x, _currentGripPoint.y, 0f);
            if (_spriteRenderer.flipX)
                point.x = -point.x;
            if (_spriteRenderer.flipY)
                point.y = -point.y;
            position = _visualTransform.TransformPoint(point);
            return true;
        }

        /// <summary>
        /// Right-facing weapon angle for the currently displayed attack frame.
        /// The weapon presenter mirrors it once, just as it does its legacy angle.
        /// </summary>
        public bool TryGetAttackWeaponAngle(out float angle)
        {
            angle = 0f;
            if (!_initialized || !isActiveAndEnabled || !_hasAttackPack ||
                _health.IsDead || _input.IsGameplayBlocked || !_rigidbody.simulated ||
                !_attack.isActiveAndEnabled || _displayedAttackPhase == PlayerAttack.AttackPhase.Idle ||
                _displayedAttackPhase != _attack.Phase)
                return false;

            angle = _attackWeaponAngle;
            return true;
        }

        private void ApplyRunPose(float speed)
        {
            _displayedAttackPhase = PlayerAttack.AttackPhase.Idle;
            if (!_running)
            {
                _running = true;
                _runFrameClock = 0f;
            }
            else
            {
                _runFrameClock = Mathf.Repeat(
                    _runFrameClock + Time.deltaTime * RunFramesPerSecond * speed / ReferenceMoveSpeed,
                    RunFrameCount);
            }

            _breathClock = 0f;
            EaseVisualScale(_restScale);
            int frame = Mathf.Clamp(Mathf.FloorToInt(_runFrameClock), 0, RunFrameCount - 1);
            _spriteRenderer.sprite = _runFrames[frame];
            _currentGripPoint = _runGripPoints[frame];
        }

        private void ApplyIdlePose()
        {
            _displayedAttackPhase = PlayerAttack.AttackPhase.Idle;
            bool stoppedRunning = _running;
            _running = false;
            _runFrameClock = 0f;
            _spriteRenderer.sprite = _idleSprite;
            _currentGripPoint = _idleGripPoint;

            // A stopped stride first returns to the authored foot-pivot scale;
            // breathing starts at zero instead of resuming halfway through an inhale.
            if (stoppedRunning)
                _breathClock = 0f;
            else
                _breathClock = Mathf.Repeat(_breathClock + Time.deltaTime, BreathDuration);

            float breath = 0.5f - 0.5f * Mathf.Cos(_breathClock / BreathDuration * Mathf.PI * 2f);
            EaseVisualScale(Vector3.Scale(_restScale,
                new Vector3(1f + BreathWidth * breath, 1f + BreathHeight * breath, 1f)));
        }

        private void EaseVisualScale(Vector3 target)
        {
            // Frame-rate independent settling prevents the now-visible inhale from
            // snapping to zero when a stride starts. Feet remain at the sprite pivot
            // and the melee presenter follows the transformed fist automatically.
            float blend = 1f - Mathf.Exp(-Time.deltaTime / ScaleResponseTime);
            _visualTransform.localScale = Vector3.Lerp(_visualTransform.localScale, target, blend);
        }

        private void ApplyNeutralPose()
        {
            _displayedAttackPhase = PlayerAttack.AttackPhase.Idle;
            _running = false;
            _runFrameClock = 0f;
            _breathClock = 0f;
            _spriteRenderer.sprite = _idleSprite;
            _currentGripPoint = _idleGripPoint;
            _visualTransform.localScale = _restScale;
        }

        private void ApplyActionPose(Sprite[] frames, Vector2[] gripPoints, int frame, bool hasPack)
        {
            _displayedAttackPhase = PlayerAttack.AttackPhase.Idle;
            // Incomplete optional art never disables the already-working base pack.
            if (!hasPack)
            {
                ApplyNeutralPose();
                return;
            }

            _running = false;
            _runFrameClock = 0f;
            _breathClock = 0f;
            EaseVisualScale(_restScale);
            // All pose packs are validated before use, but a float-derived index
            // can still land on the upper endpoint through rounding.
            frame = Mathf.Clamp(frame, 0, Mathf.Min(frames.Length, gripPoints.Length) - 1);
            _spriteRenderer.sprite = frames[frame];
            _currentGripPoint = gripPoints[frame];
        }

        private void OnJumped()
        {
            _jumpLatched = true;
            _acceptedJumpFlight = true;
            _lastLivingGround = null;
            // LateUpdate chooses priority, so a jump cannot overwrite an air attack.
        }

        private void ResetTransientState()
        {
            ClearPresentationOffset();
            _crouchWalkClock = 0f;
            if (_deathPrepared && _visualTransform != null)
                _visualTransform.localPosition = _deathOriginLocalPosition;
            _deathPrepared = false;
            _groundDeath = false;
            _activeDeathFrames = null;
            _lastLivingGround = null;
            _deathGround = null;
            _jumpLatched = false;
            _acceptedJumpFlight = false;
            _hasGroundSample = false;
            ApplyNeutralPose();
        }

        private void OnDied()
        {
            ClearPresentationOffset();
            if (!_hasDeathPack)
            {
                ResetTransientState();
                return;
            }

            _deathPrepared = true;
            // SuspendForDeath preserves IsCrouching (only respawn restores standing).
            // Latch this once: releasing crouch after the lethal hit cannot change
            // the selected death or insert an upright pose into a low collapse.
            _activeDeathFrames = _mobility != null && _mobility.IsCrouching &&
                HasCompleteDeathFrames(_crouchDeathFrames) ? _crouchDeathFrames : _deathFrames;
            _deathOriginLocalPosition = _visualTransform.localPosition;
            _deathFallStartProgress = 0f;
            _deathGround = _lastLivingGround as BoxCollider2D;
            _visualTransform.localScale = _restScale;
            _groundDeath = HasSafeDeathFloor();
            if (_groundDeath)
                _deathSupportBounds = _deathGround.bounds;
            _hasGroundSample = false;
            _jumpLatched = false;
            _acceptedJumpFlight = false;
            _running = false;
            _runFrameClock = 0f;
            _breathClock = 0f;
            _displayedAttackPhase = PlayerAttack.AttackPhase.Idle;
            ApplyDeathPose();
        }

        private void ApplyDeathPose()
        {
            // Complete every death pose even over a pit. Only the visual sinks;
            // dead-player physics, input and camera target remain suspended.
            if (_groundDeath && !IsDeathFloorUnchanged())
            {
                _groundDeath = false;
                _deathFallStartProgress = _health.DeathProgress;
            }

            int frame = Mathf.Clamp((int)(_health.DeathProgress / DeathPosesEnd * _activeDeathFrames.Length),
                0, _activeDeathFrames.Length - 1);
            _spriteRenderer.sprite = _activeDeathFrames[frame];
            _visualTransform.localScale = _restScale;
            float fall = _groundDeath ? 0f : Mathf.InverseLerp(_deathFallStartProgress, 1f, _health.DeathProgress);
            Vector3 worldDrop = Vector3.down * (0.85f * fall * fall);
            _visualTransform.localPosition = _deathOriginLocalPosition +
                _visualTransform.parent.InverseTransformVector(worldDrop);
        }

        private bool HasSafeDeathFloor()
        {
            if (_deathGround == null || !_deathGround.enabled ||
                !_deathGround.gameObject.activeInHierarchy || _deathGround.isTrigger ||
                _deathGround.edgeRadius > 0.001f)
                return false;

            Rigidbody2D groundBody = _deathGround.attachedRigidbody;
            if (groundBody != null && groundBody.bodyType != RigidbodyType2D.Static)
                return false;

            // Bounds describe a solid flat top only for an axis-aligned box. Other
            // ground shapes conservatively use recoil until they have a death policy.
            Transform floor = _deathGround.transform;
            if (Mathf.Abs(floor.right.y) > 0.001f || Mathf.Abs(floor.up.x) > 0.001f ||
                Mathf.Abs(floor.forward.z) < 0.999f)
                return false;

            Bounds support = _deathGround.bounds;
            if (Mathf.Abs(_visualTransform.position.y - support.max.y) > DeathFloorTolerance)
                return false;

            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float facing = _spriteRenderer.flipX ? -1f : 1f;
            foreach (Sprite frame in _activeDeathFrames)
            {
                float a = _visualTransform.TransformPoint(new Vector3(frame.bounds.min.x * facing, 0f, 0f)).x;
                float b = _visualTransform.TransformPoint(new Vector3(frame.bounds.max.x * facing, 0f, 0f)).x;
                minX = Mathf.Min(minX, Mathf.Min(a, b));
                maxX = Mathf.Max(maxX, Mathf.Max(a, b));
            }

            return minX >= support.min.x + DeathFloorTolerance &&
                   maxX <= support.max.x - DeathFloorTolerance;
        }

        private bool IsDeathFloorUnchanged()
        {
            if (_deathGround == null || !_deathGround.enabled ||
                !_deathGround.gameObject.activeInHierarchy || _deathGround.isTrigger)
                return false;

            Bounds current = _deathGround.bounds;
            return (current.center - _deathSupportBounds.center).sqrMagnitude < 0.0001f &&
                   (current.size - _deathSupportBounds.size).sqrMagnitude < 0.0001f;
        }

        private void OnDisable()
        {
            if (!_initialized)
                return;

            ClearPresentationOffset();

            if (_jump != null)
                _jump.Jumped -= OnJumped;
            if (_health != null)
                _health.Died -= OnDied;
            if (_respawn != null)
                _respawn.Respawned -= ResetTransientState;

            _displayedAttackPhase = PlayerAttack.AttackPhase.Idle;
            _running = false;
            _jumpLatched = false;
            _acceptedJumpFlight = false;
            _hasGroundSample = false;
            if (_deathPrepared && _visualTransform != null)
                _visualTransform.localPosition = _deathOriginLocalPosition;
            _deathPrepared = false;
            _groundDeath = false;
            _lastLivingGround = null;
            _deathGround = null;
            _runFrameClock = 0f;
            _breathClock = 0f;
            _currentGripPoint = _idleGripPoint;
            if (_spriteRenderer != null)
                _spriteRenderer.sprite = _restSprite;
            if (_visualTransform != null)
                _visualTransform.localScale = _restScale;
        }

        private bool HasRequiredReferences()
        {
            if (_input == null || _rigidbody == null || _jump == null || _attack == null ||
                _health == null || _respawn == null || _spriteRenderer == null || _idleSprite == null ||
                _spriteRenderer.transform == transform || !_spriteRenderer.transform.IsChildOf(transform) ||
                _runFrames == null || _runFrames.Length != RunFrameCount ||
                _runGripPoints == null || _runGripPoints.Length != RunFrameCount || !IsFinite(_idleGripPoint))
                return false;

            for (int i = 0; i < RunFrameCount; i++)
                if (_runFrames[i] == null || !IsFinite(_runGripPoints[i]))
                    return false;

            return true;
        }

        private static bool IsFinite(Vector2 point)
        {
            return !float.IsNaN(point.x) && !float.IsInfinity(point.x) &&
                !float.IsNaN(point.y) && !float.IsInfinity(point.y);
        }

        private static bool HasActionPack(Sprite[] frames, Vector2[] gripPoints)
        {
            if (frames == null || frames.Length != ActionFrameCount ||
                gripPoints == null || gripPoints.Length != ActionFrameCount)
                return false;
            for (int i = 0; i < ActionFrameCount; i++)
                if (frames[i] == null || !IsFinite(gripPoints[i]))
                    return false;
            return true;
        }

        private bool HasDeathPack()
        {
            return HasCompleteDeathFrames(_deathFrames);
        }

        private static bool HasCompleteDeathFrames(Sprite[] frames)
        {
            if (frames == null || frames.Length != DeathFrameCount)
                return false;
            foreach (Sprite frame in frames)
                if (frame == null)
                    return false;
            return true;
        }

        private bool HasCrouchWalkPack()
        {
            if (_crouchWalkFrames == null || _crouchWalkFrames.Length < 4 ||
                _crouchWalkGripPoints == null || _crouchWalkGripPoints.Length != _crouchWalkFrames.Length)
                return false;
            for (int i = 0; i < _crouchWalkFrames.Length; i++)
                if (_crouchWalkFrames[i] == null || !IsFinite(_crouchWalkGripPoints[i]))
                    return false;
            return true;
        }

        private bool ApplyHurtSequence()
        {
            Sprite[] frames = _mobility.IsCrouching ? _crouchHurtFrames : _hurtFrames;
            Vector2[] grips = _mobility.IsCrouching ? _crouchHurtGripPoints : _hurtGripPoints;
            if (!HasActionPack(frames, grips)) return false;
            float progress = _mobility.HurtProgress;
            int frame = progress < 0.22f ? 0 : progress < 0.57f ? 1 : 2;
            ApplyActionPose(frames, grips, frame, true);
            // Continuous, visual-only recoil. Feet/collider and camera root do not
            // move; the held weapon follows the same transformed hand anchor.
            float recoil = progress < 0.25f
                ? Mathf.SmoothStep(0f, 1f, progress / 0.25f)
                : 1f - Mathf.SmoothStep(0f, 1f, (progress - 0.25f) / 0.75f);
            float facing = _spriteRenderer.flipX ? -1f : 1f;
            Vector3 worldOffset = Vector3.left * (facing * 0.14f * recoil);
            _presentationOffset = _visualTransform.parent.InverseTransformVector(worldOffset);
            _visualTransform.localPosition += _presentationOffset;
            return true;
        }

        private void ClearPresentationOffset()
        {
            if (_visualTransform != null) _visualTransform.localPosition -= _presentationOffset;
            _presentationOffset = Vector3.zero;
        }
    }
}

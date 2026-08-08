using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Turns player state into presentation: which way the character faces and what
    /// the Animator is told. Holds no gameplay rules of its own, so movement, jumping
    /// and combat behave identically with or without this component.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerJump))]
    [RequireComponent(typeof(PlayerAttack))]
    public sealed class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Transform _attackOrigin;

        // Hashed once because the names are written to the Animator every frame.
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private static readonly int VerticalVelocityParameter = Animator.StringToHash("VerticalVelocity");
        private static readonly int IsGroundedParameter = Animator.StringToHash("IsGrounded");
        private static readonly int AttackParameter = Animator.StringToHash("Attack");

        // Below this the stick is at rest, so the last direction is kept instead of
        // being flicked around by analogue drift.
        private const float FacingInputThreshold = 0.01f;

        private PlayerInputReader _input;
        private PlayerJump _jump;
        private PlayerAttack _attack;
        private Vector3 _attackOriginRightLocalPosition;
        private bool _isFacingRight;

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();
            _jump = GetComponent<PlayerJump>();
            _attack = GetComponent<PlayerAttack>();

            if (!HasRequiredReferences())
            {
                enabled = false;
                return;
            }

            // The authored offset is stored as a reach rather than a position, so the
            // origin mirrors correctly no matter which side it was placed on. An
            // unflipped sprite is the right-facing pose.
            Vector3 authoredLocalPosition = _attackOrigin.localPosition;
            _attackOriginRightLocalPosition = new Vector3(
                Mathf.Abs(authoredLocalPosition.x),
                authoredLocalPosition.y,
                authoredLocalPosition.z);

            _isFacingRight = !_spriteRenderer.flipX;
            ApplyFacing();
        }

        private void OnEnable()
        {
            _attack.Attacked += OnAttacked;
        }

        private void OnDisable()
        {
            _attack.Attacked -= OnAttacked;
        }

        private void Update()
        {
            UpdateFacing();
            UpdateAnimator();
        }

        private void UpdateFacing()
        {
            float horizontal = _input.Horizontal;

            // Facing follows intent rather than velocity, so a standing player keeps
            // the last direction instead of turning as external forces push it around.
            if (Mathf.Abs(horizontal) < FacingInputThreshold)
            {
                return;
            }

            bool shouldFaceRight = horizontal > 0f;
            if (shouldFaceRight == _isFacingRight)
            {
                return;
            }

            _isFacingRight = shouldFaceRight;
            ApplyFacing();
        }

        private void ApplyFacing()
        {
            _spriteRenderer.flipX = !_isFacingRight;

            // Only the local X offset is mirrored: the reach keeps its authored height
            // and no negative scale is ever applied, which would deform the colliders.
            Vector3 mirroredLocalPosition = _attackOriginRightLocalPosition;
            if (!_isFacingRight)
            {
                mirroredLocalPosition.x = -mirroredLocalPosition.x;
            }

            _attackOrigin.localPosition = mirroredLocalPosition;
        }

        private void UpdateAnimator()
        {
            Vector2 velocity = _rigidbody.linearVelocity;

            // Measured speed rather than input, so running into a wall reads as idle.
            _animator.SetFloat(SpeedParameter, Mathf.Abs(velocity.x));
            _animator.SetFloat(VerticalVelocityParameter, velocity.y);
            _animator.SetBool(IsGroundedParameter, _jump.IsGrounded);
        }

        private void OnAttacked()
        {
            _animator.SetTrigger(AttackParameter);
        }

        private bool HasRequiredReferences()
        {
            // Fail once and loudly instead of throwing on every frame.
            if (_spriteRenderer == null || _animator == null || _rigidbody == null || _attackOrigin == null)
            {
                Debug.LogError(
                    $"{nameof(PlayerVisual)} needs Sprite Renderer, Animator, Rigidbody 2D and Attack Origin assigned.",
                    this);
                return false;
            }

            // Writing parameters without a controller warns on every single frame,
            // which would bury every other message in the console.
            if (_animator.runtimeAnimatorController == null)
            {
                Debug.LogError(
                    $"{nameof(PlayerVisual)} needs an Animator Controller with Speed, VerticalVelocity, IsGrounded and Attack parameters.",
                    this);
                return false;
            }

            return true;
        }
    }
}

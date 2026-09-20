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
        [SerializeField] private bool _playLegacyAttackAnimation = true;

        // Hashed once because the names are written to the Animator every frame.
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private static readonly int VerticalVelocityParameter = Animator.StringToHash("VerticalVelocity");
        private static readonly int IsGroundedParameter = Animator.StringToHash("IsGrounded");
        private static readonly int AttackParameter = Animator.StringToHash("Attack");

        private PlayerInputReader _input;
        private PlayerJump _jump;
        private PlayerAttack _attack;

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
            if (_input.IsGameplayBlocked)
                return;

            ApplyFacing();
            UpdateAnimator();
        }

        public void ResetTransientState()
        {
            if (!isActiveAndEnabled || _animator == null || !_animator.isActiveAndEnabled ||
                _animator.runtimeAnimatorController == null)
                return;

            _animator.ResetTrigger(AttackParameter);
            _animator.Rebind();
            _animator.Update(0f);
            UpdateAnimator();
            ApplyFacing();
        }

        private void ApplyFacing()
        {
            // The gameplay component owns direction and reach. Disabling rendering
            // must never change which side an attack can hit.
            _spriteRenderer.flipX = _attack.FacingDirection < 0;
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
            // The prototype's procedural spatula uses the gameplay phase clock.
            // Old scenes may retain their placeholder body-attack animation.
            if (_playLegacyAttackAnimation)
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

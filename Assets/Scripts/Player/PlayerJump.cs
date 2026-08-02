using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerJump : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _jumpForce = 10f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField, Min(0f)] private float _groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask _groundLayers;

        private Rigidbody2D _rigidbody;
        private PlayerInputReader _input;
        private bool _jumpRequested;
        private bool _awaitingTakeoff;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();

            if (_groundCheck == null)
            {
                // Fail once and loudly instead of throwing a NullReferenceException
                // on every physics step.
                Debug.LogError($"{nameof(PlayerJump)} needs a Ground Check transform assigned.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            // Latching in Update guarantees a press landing between two physics steps
            // is still seen by FixedUpdate.
            if (_input.JumpPressedThisFrame)
            {
                _jumpRequested = true;
            }
        }

        private void FixedUpdate()
        {
            bool isGrounded = IsGrounded();

            // The feet still overlap the ground for a few steps after takeoff, so the
            // request is only re-armed once the player has genuinely left the ground.
            // The velocity check releases the flag if a weak jump never lifted off.
            if (_awaitingTakeoff && (!isGrounded || _rigidbody.linearVelocity.y <= 0f))
            {
                _awaitingTakeoff = false;
            }

            if (!_jumpRequested)
            {
                return;
            }

            // Consumed even when the jump is rejected: keeping an airborne request alive
            // until landing would be a jump buffer, which is out of scope for this sprint.
            _jumpRequested = false;

            if (!isGrounded || _awaitingTakeoff)
            {
                return;
            }

            // Assigning the vertical velocity instead of adding force keeps the jump
            // height identical no matter how fast the player was falling on contact.
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);
            _awaitingTakeoff = true;
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayers) != null;
        }

        private void OnDrawGizmosSelected()
        {
            if (_groundCheck == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
        }
    }
}

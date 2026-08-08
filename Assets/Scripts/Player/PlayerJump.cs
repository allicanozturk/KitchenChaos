using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerJump : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _jumpForce = 10f;
        [SerializeField, Min(0f)] private float _coyoteTime = 0.12f;
        [SerializeField, Min(0f)] private float _jumpBufferTime = 0.15f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField, Min(0f)] private float _groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask _groundLayers;

        /// <summary>
        /// Ground state as of the last physics step. Published so presentation can
        /// read it instead of running a second overlap query of its own.
        /// </summary>
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// What the player is standing on as of the last physics step, or null while
        /// airborne. Published for the same reason as <see cref="IsGrounded"/>: the
        /// ground is already known here, so nothing else has to query for it again.
        /// </summary>
        public Collider2D GroundCollider { get; private set; }

        private Rigidbody2D _rigidbody;
        private PlayerInputReader _input;
        private bool _jumpPressLatched;
        private bool _awaitingTakeoff;
        private float _coyoteTimeRemaining;
        private float _jumpBufferRemaining;

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
            // is still seen by FixedUpdate, even when the buffer window is zero.
            if (_input.JumpPressedThisFrame)
            {
                _jumpPressLatched = true;
            }
        }

        private void FixedUpdate()
        {
            UpdateGroundContact();

            UpdateTakeoffState(IsGrounded);
            UpdateCoyoteTime(IsGrounded);

            bool hasJumpRequest = UpdateJumpBuffer();
            if (!hasJumpRequest)
            {
                return;
            }

            // Coyote time only extends the last ground contact, and Jump() spends it,
            // so it can never stack into a second jump while airborne.
            if (_awaitingTakeoff || (!IsGrounded && _coyoteTimeRemaining <= 0f))
            {
                return;
            }

            Jump();
        }

        private void UpdateTakeoffState(bool isGrounded)
        {
            // The feet still overlap the ground for a few steps after takeoff, so the
            // jump is only re-armed once the player has genuinely left the ground.
            // The velocity check releases the flag if a weak jump never lifted off.
            if (_awaitingTakeoff && (!isGrounded || _rigidbody.linearVelocity.y <= 0f))
            {
                _awaitingTakeoff = false;
            }
        }

        private void UpdateCoyoteTime(bool isGrounded)
        {
            // Refilling while the feet still overlap the ground right after a jump
            // would hand the player a free second jump in mid-air.
            if (isGrounded && !_awaitingTakeoff)
            {
                _coyoteTimeRemaining = _coyoteTime;
                return;
            }

            _coyoteTimeRemaining = Mathf.Max(0f, _coyoteTimeRemaining - Time.fixedDeltaTime);
        }

        /// <summary>
        /// Refreshes the buffer window and reports whether a jump is requested this step.
        /// </summary>
        private bool UpdateJumpBuffer()
        {
            bool pressedThisStep = _jumpPressLatched;
            _jumpPressLatched = false;

            // Fixed time is used because the window is only ever read from FixedUpdate.
            _jumpBufferRemaining = pressedThisStep
                ? _jumpBufferTime
                : Mathf.Max(0f, _jumpBufferRemaining - Time.fixedDeltaTime);

            // A fresh press is honoured on the step it arrives, so tuning the buffer
            // down to zero removes the assistance without breaking a normal jump.
            return pressedThisStep || _jumpBufferRemaining > 0f;
        }

        private void Jump()
        {
            // Assigning the vertical velocity instead of adding force keeps the jump
            // height identical no matter how fast the player was falling on contact.
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);

            // Both windows are spent so neither leftover buffered input nor remaining
            // coyote time can feed a second jump before the player lands again.
            _jumpBufferRemaining = 0f;
            _coyoteTimeRemaining = 0f;
            _awaitingTakeoff = true;
        }

        private void UpdateGroundContact()
        {
            // The same single query answers both questions, so keeping the collider
            // costs nothing on top of the ground test that already runs every step.
            GroundCollider = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayers);
            IsGrounded = GroundCollider != null;
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

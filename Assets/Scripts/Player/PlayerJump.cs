using System;
using System.Collections.Generic;
using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    [DefaultExecutionOrder(-70)]
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

        /// <summary>
        /// Raised once per accepted takeoff, so presentation reacts to the jumps that
        /// actually happened rather than to every press the rules turned down.
        /// </summary>
        public event Action Jumped;

        private Rigidbody2D _rigidbody;
        private PlayerInputReader _input;
        private bool _jumpPressLatched;
        private bool _awaitingTakeoff;
        private float _coyoteTimeRemaining;
        private float _jumpBufferRemaining;
        private ContactFilter2D _groundFilter;
        private readonly List<ContactPoint2D> _groundContacts = new(8);
        private PlayerMobility _mobility;
        private bool _airJumpAvailable;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();
            _mobility = GetComponent<PlayerMobility>();
            _groundFilter.useTriggers = false;
            _groundFilter.SetLayerMask(_groundLayers);

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
            if (_input.IsGameplayBlocked)
                return;

            // Latching in Update guarantees a press landing between two physics steps
            // is still seen by FixedUpdate, even when the buffer window is zero.
            if (_input.JumpPressedThisFrame)
            {
                _jumpPressLatched = true;
            }
        }

        private void FixedUpdate()
        {
            if (_input.IsGameplayBlocked || !_rigidbody.simulated)
                return;

            UpdateGroundContact();

            UpdateTakeoffState(IsGrounded);
            UpdateCoyoteTime(IsGrounded);
            if (IsGrounded && !_awaitingTakeoff) _airJumpAvailable = true;

            bool hasJumpRequest = UpdateJumpBuffer();
            bool upgraded = _mobility != null && _mobility.isActiveAndEnabled;
            if (upgraded && (_mobility.IsDashing || _mobility.IsParried))
            {
                _jumpBufferRemaining = 0f;
                return;
            }
            if (hasJumpRequest && !_awaitingTakeoff &&
                (!upgraded || _mobility.TryStandForJump()))
            {
                if (IsGrounded || _coyoteTimeRemaining > 0f) Jump(_jumpForce);
                else if (upgraded && _airJumpAvailable)
                {
                    _airJumpAvailable = false;
                    Jump(_jumpForce * _mobility.AirJumpMultiplier);
                }
            }
        }

        public void ResetTransientState()
        {
            _jumpPressLatched = false;
            _awaitingTakeoff = false;
            _coyoteTimeRemaining = 0f;
            _jumpBufferRemaining = 0f;
            IsGrounded = false;
            GroundCollider = null;
            _airJumpAvailable = false;
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

        private void Jump(float force)
        {
            // Assigning the vertical velocity instead of adding force keeps the jump
            // height identical no matter how fast the player was falling on contact.
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, force);

            // Both windows are spent so neither leftover buffered input nor remaining
            // coyote time can feed a second jump before the player lands again.
            _jumpBufferRemaining = 0f;
            _coyoteTimeRemaining = 0f;
            _awaitingTakeoff = true;
            IsGrounded = false;
            GroundCollider = null;

            // Announced after the windows are spent, so a listener can never observe a
            // half-applied jump state.
            Jumped?.Invoke();
        }

        private void UpdateGroundContact()
        {
            // A foot overlap also sees the SIDE of a step. Require an actual
            // upward supporting contact so walls cannot refill coyote time or
            // incorrectly hand ownership to a nearby moving platform.
            GroundCollider = null;
            int count = _rigidbody.GetContacts(_groundFilter, _groundContacts);
            float bestNormalY = 0.7f;
            for (int i = 0; i < count; i++)
            {
                ContactPoint2D contact = _groundContacts[i];
                if (contact.normal.y < bestNormalY ||
                    contact.point.y > _rigidbody.worldCenterOfMass.y)
                    continue;

                Collider2D support = contact.collider;
                if (support != null && support.attachedRigidbody == _rigidbody)
                    support = contact.otherCollider;
                if (support == null || support.isTrigger || support.attachedRigidbody == _rigidbody ||
                    (_groundLayers.value & (1 << support.gameObject.layer)) == 0)
                    continue;

                GroundCollider = support;
                bestNormalY = contact.normal.y;
            }
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

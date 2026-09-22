using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    [DefaultExecutionOrder(-50)]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;

        private Rigidbody2D _rigidbody;
        private PlayerInputReader _input;
        private PlayerMobility _mobility;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();
            _mobility = GetComponent<PlayerMobility>();
        }

        private void FixedUpdate()
        {
            if (_input.IsGameplayBlocked || !_rigidbody.simulated)
                return;

            // Jump/gravity own vertical speed except for the short horizontal dash.
            float speed = _input.Horizontal * _moveSpeed;
            if (_mobility != null && _mobility.isActiveAndEnabled)
                speed = _mobility.IsParried ? _mobility.ParryVelocity :
                    _mobility.IsDashing ? _mobility.DashVelocity :
                    _mobility.IsCrouching ? speed * 0.35f : speed;
            bool dashing = _mobility != null && _mobility.isActiveAndEnabled && _mobility.IsDashing;
            _rigidbody.linearVelocity = new Vector2(speed, dashing ? 0f : _rigidbody.linearVelocity.y);
        }
    }
}

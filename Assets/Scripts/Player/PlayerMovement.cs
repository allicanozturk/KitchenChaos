using KitchenChaos.Input;
using UnityEngine;

namespace KitchenChaos.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;

        private Rigidbody2D _rigidbody;
        private PlayerInputReader _input;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInputReader>();
        }

        private void FixedUpdate()
        {
            // Vertical velocity belongs to gravity and PlayerJump, so only the
            // horizontal axis is overwritten here.
            _rigidbody.linearVelocity = new Vector2(_input.Horizontal * _moveSpeed, _rigidbody.linearVelocity.y);
        }
    }
}

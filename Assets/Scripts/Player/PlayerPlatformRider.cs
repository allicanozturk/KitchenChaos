using KitchenChaos.Level;
using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Carries the player along with a moving platform it is standing on.
    /// Kept on the player rather than on the platform so the platform stays pure
    /// movement, and so riding costs nothing anywhere else in the movement stack:
    /// PlayerMovement keeps full control of the horizontal velocity and gravity keeps
    /// full control of the vertical one.
    /// </summary>
    // Runs after the default order so the ground contact PlayerJump resolves this
    // step is the one used to decide what the player is riding.
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerJump))]
    public sealed class PlayerPlatformRider : MonoBehaviour
    {
        // The carry offset is one step long, so a couple of iterations are enough to
        // round a corner; anything higher would only spend casts.
        private const int MaxCarryIterations = 3;

        // Any surface the carry runs into is slid along rather than treated as a dead
        // stop, so the platform can never wedge the player against a wall or a slope.
        private const float SurfaceSlideAngle = 90f;

        // Every field that has to differ from a zeroed struct is assigned explicitly,
        // so the carry behaves the same no matter which defaults the Unity version's
        // SlideMovement constructor ships with.
        private readonly Rigidbody2D.SlideMovement _carryMovement = new()
        {
            maxIterations = MaxCarryIterations,
            surfaceSlideAngle = SurfaceSlideAngle,
            surfaceUp = Vector2.up,

            // The simulation already applies world gravity to this dynamic body, and
            // surface snapping would drag the player onto nearby ledges, so the slide
            // is limited to exactly the platform's offset and nothing else.
            gravity = Vector2.zero,
            surfaceAnchor = Vector2.zero,
        };

        private Rigidbody2D _rigidbody;
        private PlayerJump _jump;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _jump = GetComponent<PlayerJump>();
        }

        private void FixedUpdate()
        {
            Collider2D ground = _jump.GroundCollider;
            if (ground == null)
            {
                return;
            }

            // A platform's collider may sit on a child object, but it reports the body
            // that actually moves, so the platform is looked up from there. Standing on
            // anything else leaves the player untouched.
            Rigidbody2D groundBody = ground.attachedRigidbody;
            if (groundBody == null || !groundBody.TryGetComponent(out MovingPlatform platform))
            {
                return;
            }

            Vector2 stepDelta = platform.StepDelta;
            if (stepDelta == Vector2.zero)
            {
                return;
            }

            // Slide sweeps the player's collider along the carry offset and stops at
            // whatever it meets, so the platform can no longer shove the player into or
            // through solid geometry the way writing the position directly did.
            // It takes the movement as an argument and reports the leftover in its
            // result, so the body's own velocity is never read or written: input and
            // gravity keep behaving exactly as they do off the platform, and a jump
            // still leaves with the authored jump force.
            _rigidbody.Slide(stepDelta / Time.fixedDeltaTime, Time.fixedDeltaTime, _carryMovement);
        }
    }
}

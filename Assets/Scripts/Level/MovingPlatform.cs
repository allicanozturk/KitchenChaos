using UnityEngine;

namespace KitchenChaos.Level
{
    /// <summary>
    /// Drives a platform back and forth between two authored points and publishes how
    /// far it travelled this physics step so riders can follow it.
    /// Movement only: the platform never touches whatever is standing on it, which
    /// keeps carrying a rider's own concern.
    /// </summary>
    // Runs before the default order so a rider reading StepDelta in its own
    // FixedUpdate gets the distance of this step rather than the previous one.
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField, Min(0f)] private float _moveSpeed = 2f;

        /// <summary>
        /// Distance covered during the current physics step. Riders add this to their
        /// own position, so the carry works for horizontal, vertical and diagonal
        /// routes without the platform knowing anything about them.
        /// </summary>
        public Vector2 StepDelta { get; private set; }

        private Rigidbody2D _rigidbody;
        private Vector2 _targetPoint;
        private Vector2 _pendingPoint;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            if (_pointA == null || _pointB == null)
            {
                // Fail once and loudly instead of moving towards the world origin on
                // every physics step.
                Debug.LogError($"{nameof(MovingPlatform)} needs both route points assigned.", this);
                enabled = false;
                return;
            }

            // A dynamic body fights MovePosition with gravity and with the weight of
            // whatever rides it, which reads as a broken platform rather than a setup
            // error.
            if (_rigidbody.bodyType != RigidbodyType2D.Kinematic)
            {
                Debug.LogError($"{nameof(MovingPlatform)} needs a Kinematic Rigidbody2D.", this);
                enabled = false;
                return;
            }

            // A trigger cannot be stood on, so the platform would silently drop the
            // player through itself.
            if (!TryGetComponent(out Collider2D platformCollider) || platformCollider.isTrigger)
            {
                Debug.LogError($"{nameof(MovingPlatform)} needs a solid Collider2D with Is Trigger disabled.", this);
                enabled = false;
                return;
            }

            // The route is fixed at its authored position, so the points may be
            // parented to the platform without being dragged along as it travels.
            _targetPoint = _pointA.position;
            _pendingPoint = _pointB.position;
        }

        private void FixedUpdate()
        {
            Vector2 currentPosition = _rigidbody.position;

            // MoveTowards clamps at the target, so neither a high speed nor a long
            // physics step can carry the platform past its route point.
            Vector2 nextPosition = Vector2.MoveTowards(
                currentPosition,
                _targetPoint,
                _moveSpeed * Time.fixedDeltaTime);

            // Published before the move so a rider running later this step applies the
            // exact same offset the platform is about to apply to itself.
            StepDelta = nextPosition - currentPosition;

            _rigidbody.MovePosition(nextPosition);

            // The clamp above makes the arrival exact, so this compares equal only on
            // the step the route point is actually reached.
            if (nextPosition == _targetPoint)
            {
                SwapTarget();
            }
        }

        private void SwapTarget()
        {
            (_targetPoint, _pendingPoint) = (_pendingPoint, _targetPoint);
        }

        private void OnDrawGizmosSelected()
        {
            if (_pointA == null || _pointB == null)
            {
                return;
            }

            // The route is invisible in the Scene view otherwise, which makes placing
            // the two points guesswork.
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_pointA.position, _pointB.position);
            Gizmos.DrawWireSphere(_pointA.position, 0.15f);
            Gizmos.DrawWireSphere(_pointB.position, 0.15f);
        }
    }
}

using UnityEngine;

namespace KitchenChaos.Enemy
{
    /// <summary>
    /// Drives an enemy back and forth between two authored patrol points.
    /// Movement only, so damage, animation and future AI can be added as separate
    /// components without touching the patrol rules.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class EnemyPatrol : MonoBehaviour
    {
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField, Min(0f)] private float _patrolSpeed = 2f;

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
                Debug.LogError($"{nameof(EnemyPatrol)} needs both patrol points assigned.", this);
                enabled = false;
                return;
            }

            // A dynamic body fights MovePosition with gravity and with anything it
            // collides into, which reads as a broken patrol rather than a setup error.
            if (_rigidbody.bodyType != RigidbodyType2D.Kinematic)
            {
                Debug.LogError($"{nameof(EnemyPatrol)} needs a Kinematic Rigidbody2D.", this);
                enabled = false;
                return;
            }

            // The patrol route is fixed at its authored position, so the points may be
            // parented to the enemy without being dragged along as it walks.
            _targetPoint = _pointA.position;
            _pendingPoint = _pointB.position;
        }

        private void FixedUpdate()
        {
            // MoveTowards clamps at the target, so neither a high speed nor a long
            // physics step can carry the enemy past its patrol point.
            Vector2 nextPosition = Vector2.MoveTowards(
                _rigidbody.position,
                _targetPoint,
                _patrolSpeed * Time.fixedDeltaTime);

            _rigidbody.MovePosition(nextPosition);

            // The clamp above makes the arrival exact, so this compares equal only on
            // the step the patrol point is actually reached.
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

            // The patrol route is invisible in the Scene view otherwise, which makes
            // placing the two points guesswork.
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_pointA.position, _pointB.position);
            Gizmos.DrawWireSphere(_pointA.position, 0.15f);
            Gizmos.DrawWireSphere(_pointB.position, 0.15f);
        }
    }
}

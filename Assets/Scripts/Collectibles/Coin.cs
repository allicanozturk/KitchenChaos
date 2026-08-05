using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Collectibles
{
    /// <summary>
    /// A one-shot pickup that adds its value to the score of the player who touches it.
    /// </summary>
    public sealed class Coin : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _value = 1;

        private bool _isCollected;

        private void Awake()
        {
            // Without a trigger collider the coin is silently uncollectable, so fail
            // once and loudly instead of leaving the designer to guess.
            if (!TryGetComponent(out Collider2D coinCollider) || !coinCollider.isTrigger)
            {
                Debug.LogError($"{nameof(Coin)} needs a Collider2D with Is Trigger enabled.", this);
                enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Destroy only takes effect at the end of the frame, so a second overlap
            // in the same frame would otherwise score the same coin twice.
            if (_isCollected)
            {
                return;
            }

            // A player's colliders may sit on child objects, but they all report the
            // same attached body, so the score is looked up from that body instead.
            Rigidbody2D touchingBody = other.attachedRigidbody;
            if (touchingBody == null || !touchingBody.TryGetComponent(out PlayerScore score))
            {
                return;
            }

            _isCollected = true;
            score.Add(_value);
            Destroy(gameObject);
        }
    }
}

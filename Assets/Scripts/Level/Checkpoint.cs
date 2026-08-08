using KitchenChaos.Player;
using UnityEngine;

namespace KitchenChaos.Level
{
    /// <summary>
    /// Makes its own position the player's respawn point when the player touches it.
    /// The respawn point stays owned by <see cref="PlayerRespawn"/>, so no manager or
    /// global lookup is involved: the checkpoint only talks to the player it touches.
    /// </summary>
    public sealed class Checkpoint : MonoBehaviour
    {
        private void Awake()
        {
            // Without a trigger collider the checkpoint is silently unreachable, so fail
            // once and loudly instead of leaving the designer to guess.
            if (!TryGetComponent(out Collider2D checkpointCollider) || !checkpointCollider.isTrigger)
            {
                Debug.LogError($"{nameof(Checkpoint)} needs a Collider2D with Is Trigger enabled.", this);
                enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // A player's colliders may sit on child objects, but they all report the
            // same attached body, so the respawn is looked up from that body instead.
            // Anything without PlayerRespawn cannot activate the checkpoint.
            Rigidbody2D touchingBody = other.attachedRigidbody;
            if (touchingBody == null || !touchingBody.TryGetComponent(out PlayerRespawn respawn))
            {
                return;
            }

            // Re-entering is harmless: writing the same position again changes nothing,
            // and coming back to an earlier checkpoint is meant to reclaim it.
            respawn.SetSpawnPosition(transform.position);

#if UNITY_EDITOR
            // Checkpoints have no visual state yet, so play tests need a way to see
            // that activation happened without shipping log noise in the player build.
            Debug.Log($"Checkpoint activated: {name}", this);
#endif
        }
    }
}

using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Owns where the player reappears and puts it back there on request.
    /// Keeping this out of PlayerHealth lets checkpoints move the respawn point
    /// without touching how damage is handled.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerRespawn : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private Vector2 _spawnPosition;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            // The position authored in the scene is the spawn point, so no marker
            // object has to be created and wired up in the Inspector.
            _spawnPosition = _rigidbody.position;
        }

        /// <summary>
        /// Moves the point the player returns to after death. The latest call wins, so
        /// activating a checkpoint simply replaces the previous respawn point.
        /// </summary>
        public void SetSpawnPosition(Vector2 position)
        {
            _spawnPosition = position;
        }

        public void Respawn()
        {
            // Teleporting through the body keeps the physics and render positions in
            // sync, and clearing the velocity stops the fall speed the player died
            // with from carrying over into the new life.
            _rigidbody.position = _spawnPosition;
            _rigidbody.linearVelocity = Vector2.zero;
        }
    }
}

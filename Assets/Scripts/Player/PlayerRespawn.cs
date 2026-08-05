using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Remembers where the player started and puts it back there on request.
    /// Keeping this out of PlayerHealth lets a future checkpoint system change where
    /// the player reappears without touching how damage is handled.
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

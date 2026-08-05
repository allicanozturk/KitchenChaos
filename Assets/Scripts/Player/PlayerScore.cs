using UnityEngine;

namespace KitchenChaos.Player
{
    /// <summary>
    /// Holds the score collected by this player. Collectibles reach it through the
    /// object they physically touch, so no manager, singleton or global lookup is needed.
    /// </summary>
    public sealed class PlayerScore : MonoBehaviour
    {
        public int Current { get; private set; }

        public void Add(int amount)
        {
            Current += amount;

#if UNITY_EDITOR
            // The score has no UI yet, so play tests need a way to verify it without
            // shipping log noise in the player build.
            Debug.Log($"Score: {Current}", this);
#endif
        }
    }
}

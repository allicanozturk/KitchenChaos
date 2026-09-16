using System;
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

        /// <summary>
        /// Raised with the amount every time score is added. A collectible destroys
        /// itself on pickup, so this is where presentation can still react to it.
        /// </summary>
        public event Action<int> ScoreAdded;

        public void Add(int amount)
        {
            Current += amount;
            ScoreAdded?.Invoke(amount);

#if UNITY_EDITOR
            // The score has no UI yet, so play tests need a way to verify it without
            // shipping log noise in the player build.
            Debug.Log($"Score: {Current}", this);
#endif
        }
    }
}

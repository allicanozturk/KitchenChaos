using UnityEngine;

namespace KitchenChaos.Input
{
    /// <summary>
    /// Owns the single input asset instance for the player and publishes its values
    /// as plain data, so gameplay components never depend on the Input System API.
    /// </summary>
    // Runs before every gameplay component so the cached values are always refreshed
    // before anything reads them, without relying on undefined script execution order.
    [DefaultExecutionOrder(-100)]
    public sealed class PlayerInputReader : MonoBehaviour
    {
        public float Horizontal { get; private set; }

        public bool JumpPressedThisFrame { get; private set; }

        public bool AttackPressedThisFrame { get; private set; }

        private InputSystem_Actions _inputActions;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();

            // Consumers keep reading these properties while disabled; clearing them
            // prevents the player from drifting on the last known input.
            Horizontal = 0f;
            JumpPressedThisFrame = false;
            AttackPressedThisFrame = false;
        }

        private void OnDestroy()
        {
            // Awake never runs when the object is destroyed while still inactive.
            _inputActions?.Dispose();
        }

        private void Update()
        {
            Horizontal = _inputActions.Player.Move.ReadValue<Vector2>().x;
            JumpPressedThisFrame = _inputActions.Player.Jump.WasPressedThisFrame();
            AttackPressedThisFrame = _inputActions.Player.Attack.WasPressedThisFrame();
        }
    }
}

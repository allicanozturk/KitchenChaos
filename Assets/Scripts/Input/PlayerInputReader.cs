using UnityEngine;
using UnityEngine.InputSystem;

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
        public bool JumpHeld { get; private set; }
        public bool DashPressedThisFrame { get; private set; }
        public bool CrouchHeld { get; private set; }
        public bool RangedHeld { get; private set; }
        public bool SpecialPressedThisFrame { get; private set; }
        public Vector2 Aim { get; private set; }

        public bool IsGameplayBlocked { get; private set; }

        private InputSystem_Actions _inputActions;
        private int _suppressThroughFrame = -1;
        private bool _waitForJumpRelease;
        private bool _waitForAttackRelease;
        private bool _waitForDashRelease;
        private InputAction _dash;
        private InputAction _crouch;
        private InputAction _ranged;
        private InputAction _aim;
        private bool _waitForRangedRelease;
        private InputAction _special;
        private bool _waitForSpecialRelease;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
            // Extend this owned runtime asset without editing its generated wrapper.
            _dash = _inputActions.Player.Get().AddAction("GroundDash", InputActionType.Button);
            _dash.AddBinding("<Keyboard>/leftShift");
            _dash.AddBinding("<Gamepad>/rightShoulder");
            _crouch = _inputActions.Player.Get().AddAction("CrouchHold", InputActionType.Button);
            _crouch.AddBinding("<Keyboard>/s");
            _crouch.AddBinding("<Keyboard>/downArrow");
            _crouch.AddBinding("<Keyboard>/c");
            _crouch.AddBinding("<Gamepad>/dpad/down");
            _ranged = _inputActions.Player.Get().AddAction("ThrowFork", InputActionType.Button);
            _ranged.AddBinding("<Gamepad>/rightTrigger");
            _ranged.AddBinding("<Keyboard>/k");
            _ranged.AddBinding("<Mouse>/rightButton");
            _aim = _inputActions.Player.Get().AddAction("ForkAim", InputActionType.Value);
            _aim.AddBinding("<Gamepad>/rightStick");
            _special = _inputActions.Player.Get().AddAction("SpinSpecial", InputActionType.Button);
            _special.AddBinding("<Gamepad>/buttonNorth");
            _special.AddBinding("<Keyboard>/q");
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            ResetTransientState();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
            ResetTransientState();
        }

        private void OnDestroy()
        {
            // Awake never runs when the object is destroyed while still inactive.
            _inputActions?.Dispose();
        }

        private void Update()
        {
            if (IsGameplayBlocked || Time.frameCount <= _suppressThroughFrame)
            {
                ClearPublishedInput();
                return;
            }

            if (!_inputActions.Player.Jump.IsPressed())
                _waitForJumpRelease = false;
            if (!_inputActions.Player.Attack.IsPressed())
                _waitForAttackRelease = false;
            if (!_dash.IsPressed()) _waitForDashRelease = false;
            if (!_ranged.IsPressed()) _waitForRangedRelease = false;
            if (!_special.IsPressed()) _waitForSpecialRelease = false;

            Horizontal = _inputActions.Player.Move.ReadValue<Vector2>().x;
            JumpPressedThisFrame = !_waitForJumpRelease && _inputActions.Player.Jump.WasPressedThisFrame();
            AttackPressedThisFrame = !_waitForAttackRelease && _inputActions.Player.Attack.WasPressedThisFrame();
            JumpHeld = !_waitForJumpRelease && _inputActions.Player.Jump.IsPressed();
            DashPressedThisFrame = !_waitForDashRelease && _dash.WasPressedThisFrame();
            CrouchHeld = _crouch.IsPressed() || _inputActions.Player.Move.ReadValue<Vector2>().y < -0.5f;
            RangedHeld = !_waitForRangedRelease && _ranged.IsPressed();
            SpecialPressedThisFrame = !_waitForSpecialRelease && _special.WasPressedThisFrame();
            Vector2 stickAim = _aim.ReadValue<Vector2>();
            // A neutral right stick means forward fire, never left-stick crouch aim.
            // Retain directional keyboard throws without borrowing gamepad movement.
            var move = _inputActions.Player.Move;
            Vector2 keyboardAim = move.activeControl?.device is Keyboard
                ? move.ReadValue<Vector2>() : Vector2.zero;
            Aim = stickAim.sqrMagnitude > 0.16f ? stickAim : keyboardAim;
        }

        public void SetGameplayBlocked(bool blocked)
        {
            IsGameplayBlocked = blocked;
            ResetTransientState();
        }

        public void ResetTransientState()
        {
            ClearPublishedInput();
            _suppressThroughFrame = Time.frameCount;
            // Held action buttons must be released; new presses after respawn work
            // normally. The action map stays enabled, preserving authored settings.
            _waitForJumpRelease = _inputActions != null && _inputActions.Player.Jump.IsPressed();
            _waitForAttackRelease = _inputActions != null && _inputActions.Player.Attack.IsPressed();
            _waitForDashRelease = _dash != null && _dash.IsPressed();
            _waitForRangedRelease = _ranged != null && _ranged.IsPressed();
            _waitForSpecialRelease = _special != null && _special.IsPressed();
        }

        private void ClearPublishedInput()
        {
            Horizontal = 0f;
            JumpPressedThisFrame = false;
            AttackPressedThisFrame = false;
            JumpHeld = false;
            DashPressedThisFrame = false;
            CrouchHeld = false;
            RangedHeld = false;
            SpecialPressedThisFrame = false;
            Aim = Vector2.zero;
        }
    }
}

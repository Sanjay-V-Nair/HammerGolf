using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HammerGolf.RotationSystem {

    /// <summary>
    /// Abstraction over whatever input method drives rotation.
    /// Implement this to swap mouse, touch, gamepad, or any other scheme
    /// without changing RotationController.
    /// </summary>
    public interface IRotationInputProvider {
        /// <summary>
        /// Returns a raw, unscaled rotation delta this frame.
        /// The controller will axis-mask and speed-scale it before forwarding to the rotatable.
        /// Return Vector3.zero when there is no input.
        /// </summary>
        Vector3 GetDelta();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Default: mouse drag + scroll wheel
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Translates mouse input into a full 3-axis rotation delta using the
    /// New Input System direct API (no action assets required).
    ///
    /// Mapping:
    ///   Drag left/right  (mouse.delta.x)   → Y-axis rotation
    ///   Drag up/down     (mouse.delta.y)   → X-axis rotation
    ///   Scroll up/down   (mouse.scroll.y)  → Z-axis rotation
    ///
    /// The RotationController then masks this delta against the rotatable's
    /// RotationAxis, so a Y-only rotatable simply ignores X and Z input.
    /// </summary>
    public sealed class MouseDragInputProvider : MonoBehaviour, IRotationInputProvider {

        [Tooltip("Mouse drag movement → rotation degrees scale factor.")]
        [SerializeField, Range(0.01f, 5f)] private float _dragSensitivity = 0.5f;

        [Tooltip("Scroll wheel → Z-axis rotation degrees scale factor.")]
        [SerializeField, Range(0.01f, 5f)] private float _scrollSensitivity = 1f;

        public Vector3 GetDelta() {
            Mouse mouse = Mouse.current;
            if (mouse == null) return Vector3.zero;

            Vector3 delta = Vector3.zero;

            // X and Y from drag — only while left button is held.
            if (mouse.leftButton.isPressed) {
                Vector2 drag = mouse.delta.ReadValue();
                // Negate Y so dragging up rotates forward naturally.
                delta.x = -drag.y * _dragSensitivity;
                delta.y =  drag.x * _dragSensitivity;
            }

            // Z from scroll wheel — no button hold required.
            float scroll = mouse.scroll.ReadValue().y;
            if (scroll != 0f)
                delta.z = -scroll * _scrollSensitivity;

            return delta;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Alternative: gamepad right stick + triggers
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Uses the right analogue stick (X/Y rotation) and left/right triggers
    /// (Z rotation) of the current gamepad. Drop-in replacement for
    /// MouseDragInputProvider.
    ///
    /// Mapping:
    ///   Right stick left/right  → Y-axis rotation
    ///   Right stick up/down     → X-axis rotation
    ///   Left trigger - Right trigger (differential) → Z-axis rotation
    /// </summary>
    public sealed class GamepadStickInputProvider : MonoBehaviour, IRotationInputProvider {

        [SerializeField, Range(0.01f, 5f)] private float _stickSensitivity = 1f;
        [SerializeField, Range(0.01f, 5f)] private float _triggerSensitivity = 1f;
        [SerializeField, Range(0f, 1f)]    private float _deadzone = 0.15f;

        public Vector3 GetDelta() {
            Gamepad pad = Gamepad.current;
            if (pad == null) return Vector3.zero;

            Vector2 stick = pad.rightStick.ReadValue();
            if (Mathf.Abs(stick.x) < _deadzone) stick.x = 0f;
            if (Mathf.Abs(stick.y) < _deadzone) stick.y = 0f;

            // Left trigger − right trigger gives a signed Z value.
            float lt = pad.leftTrigger.ReadValue();
            float rt = pad.rightTrigger.ReadValue();
            float zDelta = (lt - rt) * _triggerSensitivity;

            return new Vector3(-stick.y * _stickSensitivity,
                                stick.x * _stickSensitivity,
                                zDelta);
        }
    }
}
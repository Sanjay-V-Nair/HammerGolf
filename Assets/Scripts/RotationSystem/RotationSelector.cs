using UnityEngine;
using UnityEngine.InputSystem;

namespace HammerGolf.RotationSystem {

    /// <summary>
    /// Attach ONE of these to a persistent manager GameObject (alongside RotationController).
    /// Fires one raycast per click and resolves the hit Collider against the controller's
    /// pre-built lookup table — O(1), no hierarchy traversal.
    /// </summary>
    public sealed class RotationSelector : MonoBehaviour {

        [Tooltip("Camera used for raycasting. Defaults to Camera.main.")]
        [SerializeField] private Camera _camera;

        [Tooltip("Only Colliders on these layers can activate a rotatable.")]
        [SerializeField] private LayerMask _selectableLayers = ~0;

        [Tooltip("Key that deselects the active rotatable.")]
        [SerializeField] private Key _cancelKey = Key.Escape;

        [Tooltip("Max raycast distance.")]
        [SerializeField] private float _maxDistance = Mathf.Infinity;

        private void Awake() {
            if (_camera == null) _camera = Camera.main;
        }

        private void Update() {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            if (keyboard != null && keyboard[_cancelKey].wasPressedThisFrame) {
                RotationController.Instance.SetActive(null);
                return;
            }

            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;

            // ReadValue returns screen-space position, matching Camera.ScreenPointToRay.
            Vector2 screenPos = mouse.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(screenPos);

            if (!Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _selectableLayers)) {
                RotationController.Instance.SetActive(null);
                return;
            }

            // O(1) lookup — the controller already knows which IRotatable owns this collider.
            RotationController.Instance.TryResolve(hit.collider, out IRotatable target);
            RotationController.Instance.SetActive(target); // null = deselect
        }
    }
}
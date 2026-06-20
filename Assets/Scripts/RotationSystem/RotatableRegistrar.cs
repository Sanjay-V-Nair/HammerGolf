using UnityEngine;

namespace HammerGolf.RotationSystem {

    /// <summary>
    /// Attach this to the GameObject that holds the IRotatable component (typically the parent).
    /// Bridges MonoBehaviour lifecycle (OnEnable/OnDisable) with the RotationController,
    /// and explicitly declares which Colliders represent this rotatable for hit-testing.
    ///
    /// Simple setup (IRotatable and Collider on the same object):
    ///   • Leave _colliders empty — the registrar falls back to GetComponent<Collider>() on self.
    ///
    /// Parent-child setup (e.g. a ring made of separate collider pieces):
    ///   • Add this to the PARENT alongside the IRotatable component.
    ///   • Assign each child piece's Collider in the _colliders array.
    ///   • Child pieces need no components added to them — they are pure geometry.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RotatableRegistrar : MonoBehaviour {

        [Tooltip("Colliders that represent this rotatable for click/raycast detection. " +
                 "Can live on child objects. Leave empty to auto-detect a Collider on this GameObject.")]
        [SerializeField] private Collider[] _colliders;

        [Tooltip("Override to manually assign the IRotatable component. " +
                 "Leave empty to auto-detect on this GameObject.")]
        [SerializeField] private MonoBehaviour _rotatableOverride;

        private IRotatable _rotatable;
        private Collider[] _resolvedColliders;

        private void Awake() {
            _rotatable = _rotatableOverride != null
                ? _rotatableOverride as IRotatable
                : GetComponent<IRotatable>();

            if (_rotatable == null) {
                Debug.LogError(
                    $"[RotatableRegistrar] No IRotatable found on '{gameObject.name}'. " +
                    "Add a component that implements IRotatable or assign one via the inspector.", this);
                return;
            }

            // Resolve colliders: use the explicit list if provided, else self-collider.
            if (_colliders != null && _colliders.Length > 0) {
                _resolvedColliders = _colliders;
            } else {
                var self = GetComponent<Collider>();
                if (self != null)
                    _resolvedColliders = new[] { self };
                else
                    Debug.LogWarning(
                        $"[RotatableRegistrar] No colliders assigned or found on '{gameObject.name}'. " +
                        "This rotatable will never be selectable via raycasting.", this);
            }
        }

        private void OnEnable() {
            if (_rotatable != null)
                RotationController.Instance.Register(_rotatable, _resolvedColliders);
        }

        private void OnDisable() {
            if (_rotatable != null && RotationController.HasInstance)
                RotationController.Instance.Deregister(_rotatable, _resolvedColliders);
        }
    }
}
using UnityEngine;

namespace HammerGolf.RotationSystem {

    /// <summary>
    /// Example of a custom IRotatable that is NOT a RotationRing.
    /// Shows how any MonoBehaviour can plug into the system by implementing IRotatable
    /// and adding a RotatableRegistrar + RotationSelector.
    ///
    /// This platform also carries any Rigidbody objects resting on it,
    /// demonstrating domain-specific logic living inside the IRotatable implementation.
    /// </summary>
    public class RotatingPlatform : MonoBehaviour, IRotatable {

        // ── IRotatable ────────────────────────────────────────────────────────

        [field: SerializeField] public float RotationSpeed { get; private set; } = 60f;

        // Platforms only rotate on the Z axis (rolling left/right).
        public Vector3 RotationAxis => Vector3.forward;

        public void Rotate(Vector3 delta) {
            transform.Rotate(delta, Space.World);
        }

        public void OnRotationStart() {
            Debug.Log($"[RotatingPlatform] '{name}' selected.");
            // Could tint the platform, play a sound, show UI hint, etc.
        }

        public void OnRotationEnd() {
            Debug.Log($"[RotatingPlatform] '{name}' deselected.");
        }

        // ── Platform-specific ─────────────────────────────────────────────────

        [Tooltip("Objects resting on the platform will be parented while rotating " +
                 "so they move with it.")]
        [SerializeField] private bool _carryChildren = true;

        private void OnCollisionEnter(Collision collision) {
            if (_carryChildren && RotationController.Instance.Active == (IRotatable)this)
                collision.transform.SetParent(transform);
        }

        private void OnCollisionExit(Collision collision) {
            if (collision.transform.parent == transform)
                collision.transform.SetParent(null);
        }
    }
}
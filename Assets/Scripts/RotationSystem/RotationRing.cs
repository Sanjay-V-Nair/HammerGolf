using UnityEngine;

namespace HammerGolf.RotationSystem {

    /// <summary>
    /// A ring (or any transform) that rotates around a single axis when selected.
    /// </summary>
    public class RotationRing : MonoBehaviour, IRotatable {

        // ── IRotatable ────────────────────────────────────────────────────────

        [field: Header("Rotation Settings")]
        [field: SerializeField, Tooltip("Degrees per second (scaled by global multiplier).")]
        public float RotationSpeed { get; private set; } = 90f;

        [field: SerializeField, Tooltip("Which world axes this ring can rotate on. " +
                                        "E.g. (0,1,0) = Y-axis only.")]
        public Vector3 RotationAxis { get; private set; } = Vector3.up;

        // ── Optional snap ─────────────────────────────────────────────────────

        [Header("Snapping (optional)")]
        [SerializeField] private bool _snapOnRelease = false;
        [SerializeField, Range(1f, 90f)] private float _snapDegrees = 45f;

        // ── Visual feedback ───────────────────────────────────────────────────

        [Header("Highlight")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _highlightMaterial;
        private Material _defaultMaterial;

        // ── IRotatable callbacks ──────────────────────────────────────────────

        public virtual void OnRotationStart() {
            if (_renderer != null && _highlightMaterial != null) {
                _defaultMaterial = _renderer.material;
                _renderer.material = _highlightMaterial;
            }
        }

        public virtual void OnRotationEnd() {
            if (_renderer != null && _defaultMaterial != null)
                _renderer.material = _defaultMaterial;

            if (_snapOnRelease)
                SnapToNearest();
        }

        public virtual void Rotate(Vector3 delta) {
            // delta is already axis-masked and speed-scaled by the controller.
            transform.Rotate(delta, Space.World);
        }

        // ── Snap ──────────────────────────────────────────────────────────────

        private void SnapToNearest() {
            Vector3 euler = transform.eulerAngles;

            // Only snap on the active axes.
            if (RotationAxis.x != 0) euler.x = SnapAngle(euler.x, _snapDegrees);
            if (RotationAxis.y != 0) euler.y = SnapAngle(euler.y, _snapDegrees);
            if (RotationAxis.z != 0) euler.z = SnapAngle(euler.z, _snapDegrees);

            transform.eulerAngles = euler;
        }

        private static float SnapAngle(float angle, float step) =>
            Mathf.Round(angle / step) * step;
    }
}
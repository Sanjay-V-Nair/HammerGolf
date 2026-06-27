using UnityEngine;

namespace HammerGolf.RotationSystem {
    public class DynamicRotatable : MonoBehaviour, IRotatable {
        [field: SerializeField] public float RotationSpeed { get; set; } = 60f;
        [field: SerializeField] public Vector3 RotationAxis { get; set; } = Vector3.up;

        public void Rotate(Vector3 delta) {
            transform.Rotate(delta, Space.World);
        }

        public void OnRotationStart() {
            Debug.Log($"[DynamicRotatable] '{name}' selected.");
        }

        public void OnRotationEnd() {
            Debug.Log($"[DynamicRotatable] '{name}' deselected.");
        }
    }
}

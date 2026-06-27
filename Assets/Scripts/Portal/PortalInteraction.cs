using UnityEngine;

namespace HammerGolf
{
    public class PortalInteraction : MonoBehaviour
    {
        [Header("Portal")]
        [SerializeField] PortalInteraction linkedPortal;

        [Header("Settings")]
        [SerializeField] float minimumExitSpeed = 12f;
        [SerializeField] float cooldown = 0.25f;

        private float _lastTeleportTime;

        public float Cooldown { get => cooldown; }
        public float LastTeleportTime { get => _lastTeleportTime; }

        public void SetLinkedPortal(PortalInteraction other)
        {
            linkedPortal = other;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Rigidbody rb))
                return;

            if (linkedPortal == null)
                return;

            if (Time.time - _lastTeleportTime < cooldown || Time.time - linkedPortal.LastTeleportTime < linkedPortal.Cooldown)
                return;

            Teleport(rb);
        }

        private void Teleport(Rigidbody ball)
        {
            linkedPortal._lastTeleportTime = Time.time;
            _lastTeleportTime = Time.time;

            Vector3 velocity = ball.linearVelocity;

            float speed = Mathf.Max(
                velocity.magnitude,
                minimumExitSpeed);

            Vector3 exitDirection = linkedPortal.transform.forward;

            ball.position =
                linkedPortal.transform.position +
                exitDirection * 1.2f;

            ball.linearVelocity =
                exitDirection * speed;
        }
    }
}

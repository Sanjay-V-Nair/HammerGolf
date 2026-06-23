using UnityEngine;

namespace HammerGolf
{
    public class BallThrowController : MonoBehaviour
    {
        #region Serialize Fields

        [Header("References")]
        [SerializeField] CharacterSpinController spinController;
        [SerializeField] Transform orbitPivot;

        [Header("Orbit tuning")]
        [SerializeField] float orbitRadius = 1.5f;

        [Header("Throw")]
        [Range(0f, 89f)][SerializeField] float launchAngle = 35f;
        [SerializeField] float minLaunchSpeed = 4f;
        [SerializeField] float maxLaunchSpeed = 28f;


        // ---------------------------------------------------------------
        #endregion

        #region Events

        public event System.Action<Rigidbody> OnThrown;


        // ---------------------------------------------------------------
        #endregion

        #region Cache

        Rigidbody rb;

        // ---------------------------------------------------------------
        #endregion

        void Awake()
        {
            rb = GetComponent<Rigidbody>();

            if (spinController == null)
            {
                return;
            }

            if (orbitPivot == null) orbitPivot = spinController.transform;

            spinController.OnSpinUpdated += HandleSpinUpdated;
            spinController.OnReleased += HandleReleased;
        }

        void OnDestroy()
        {
            if (spinController == null) return;
            spinController.OnSpinUpdated -= HandleSpinUpdated;
            spinController.OnReleased -= HandleReleased;
        }

        void Start()
        {
            SetOrbiting(true);
            SnapToOrbit(orbitPivot.eulerAngles.y);
        }

        void HandleSpinUpdated(float normalizedSpin, float facingAngle)
        {
            SnapToOrbit(facingAngle);
        }

        void HandleReleased(float normalizedSpin, float facingAngle)
        {
            Launch(normalizedSpin, facingAngle);
        }

        void SnapToOrbit(float facingAngle)
        {
            Vector3 dir = Quaternion.Euler(0f, facingAngle, 0f) * Vector3.forward;
            rb.MovePosition(orbitPivot.position + dir * orbitRadius);
        }

        void Launch(float normalizedSpin, float facingAngle)
        {
            float launchSpeed = Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, normalizedSpin);
            float horizontalSpeed = launchSpeed * Mathf.Cos(launchAngle * Mathf.Deg2Rad);
            float verticalSpeed = launchSpeed * Mathf.Sin(launchAngle * Mathf.Deg2Rad);

            Vector3 dir = Quaternion.Euler(0f, facingAngle, 0f) * Vector3.forward;

            SetOrbiting(false);
            var force = dir.normalized * horizontalSpeed + Vector3.up * verticalSpeed;
            rb.AddForce(force, ForceMode.VelocityChange);

            OnThrown?.Invoke(rb);
        }

        void SetOrbiting(bool orbiting)
        {
            rb.isKinematic = orbiting;
            rb.useGravity = !orbiting;
        }

        public void ResetToOrbit()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            SetOrbiting(true);
            SnapToOrbit(orbitPivot.eulerAngles.y);
        }

    } 
}

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

        Rigidbody _rb;
        Vector3 _uninitializedVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        Vector3 _lastThrownFrom = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        // ---------------------------------------------------------------
        #endregion

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();

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
            _rb.MovePosition(orbitPivot.position + dir * orbitRadius);
        }

        void Launch(float normalizedSpin, float facingAngle)
        {
            float launchSpeed = Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, normalizedSpin);
            float horizontalSpeed = launchSpeed * Mathf.Cos(launchAngle * Mathf.Deg2Rad);
            float verticalSpeed = launchSpeed * Mathf.Sin(launchAngle * Mathf.Deg2Rad);

            Vector3 dir = Quaternion.Euler(0f, facingAngle, 0f) * Vector3.forward;

            SetOrbiting(false);
            var force = dir.normalized * horizontalSpeed + Vector3.up * verticalSpeed;
            _rb.AddForce(force, ForceMode.VelocityChange);

            OnThrown?.Invoke(_rb);

            _lastThrownFrom = spinController.transform.position;
        }

        void SetOrbiting(bool orbiting)
        {
            _rb.isKinematic = orbiting;
            _rb.useGravity = !orbiting;
        }

        public void ResetToOrbit()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            SetOrbiting(true);
            SnapToOrbit(orbitPivot.eulerAngles.y);
        }

        public void ReturnToLastShot()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            spinController.transform.position = _lastThrownFrom;
            transform.position = _lastThrownFrom;

            SetOrbiting(true);
        }
    } 
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace HammerGolf
{
    public class CharacterSpinController : MonoBehaviour
    {
        #region SerializedFields

        [Header("Spin tuning")]
        [SerializeField] float spinBuildRate = 25f;
        [SerializeField] float spinDecayRate = 60f;
        [SerializeField] float maxSpinSpeed = 720f;

        [Header("References")]
        [SerializeField] CharacterRecoveryController recoveryController;

        // ---------------------------------------------------------------
        #endregion

        #region Events

        public event System.Action ChargeStarted;

        // arg 1: normalized spin speed (0-1) | arg 2: facing angle (degrees)
        public event System.Action<float, float> OnSpinUpdated;
        public event System.Action<float, float> OnReleased;


        // ---------------------------------------------------------------

        #endregion

        #region Cache

        public bool IsCharging { get => spinState == SpinState.Charging; }
        public bool CanCharge { get => spinState == SpinState.Idle; }
        CameraEventsController _camEvents => CameraEventsController.Instance;

        SpinState spinState;

        float spinSpeed;
        float facingAngle;

        CharacterInputActions input => InputManager.Instance.CharInputActions;

        // ---------------------------------------------------------------

        #endregion

        void Awake()
        {
            facingAngle = transform.eulerAngles.y;
            spinState = SpinState.Idle;
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void Start()
        {
            recoveryController.RecoveryComplete += HandleRecoveryComplete;
        }

        private void OnDestroy()
        {
            recoveryController.RecoveryComplete -= HandleRecoveryComplete;
        }

        void Update()
        {
            Debug.Log($"Current spin state: {spinState}");

            if (!IsCharging && CanCharge)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    BeginCharge();
                }
                return;
            }

            if (IsCharging)
            {
                Charge();
            }

            if (IsCharging && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                EndCharge();
            }
        }

        void BeginCharge()
        {
            spinState = SpinState.Charging;
            spinSpeed = 0f;
            ChargeStarted?.Invoke();
        }

        void Charge()
        {
            float speedInput = input.Gameplay.SpinSpeed.ReadValue<float>();
            Debug.Log($"Speed charInputActions value: {speedInput}");

            spinSpeed += speedInput * spinBuildRate * Time.deltaTime;
            spinSpeed = Mathf.Clamp(spinSpeed, -maxSpinSpeed, maxSpinSpeed);

            if (speedInput == 0)
            {
                spinSpeed -= spinDecayRate * Time.deltaTime;
                spinSpeed = Mathf.Clamp(spinSpeed, 0f, maxSpinSpeed);
            }

            facingAngle = (facingAngle + spinSpeed * Time.deltaTime) % 360f;
            transform.rotation = Quaternion.Euler(0f, facingAngle, 0f);

            OnSpinUpdated?.Invoke(spinSpeed / maxSpinSpeed, facingAngle);
        }

        void EndCharge()
        {
            spinState = SpinState.Recovery;
            OnReleased?.Invoke(spinSpeed / maxSpinSpeed, facingAngle);
            CameraEventsController.SwitchToBall?.Invoke(this, null);
        }

        void HandleRecoveryComplete()
        {
            spinState = SpinState.Idle;
        }
    } 
}


using Unity.VisualScripting;
using UnityEngine;

namespace HammerGolf
{
    public class InputManager : MonoBehaviour
    {

        #region Cached Inputs

        CharacterInputActions charInputActions;

        #endregion

        public static InputManager Instance { get; private set; }

        public CharacterInputActions CharInputActions => charInputActions;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            InitializeInputs();
        }

        private void OnEnable()
        {
            EnableInputs();
        }

        private void OnDisable()
        {
            DisableInputs();
        }

        private void OnDestroy()
        {
            charInputActions.Disable();
        }

        void InitializeInputs()
        {
            charInputActions = new CharacterInputActions();
        }

        void EnableInputs()
        {
            charInputActions.Enable();
        }

        void DisableInputs()
        {
            charInputActions.Disable();
        }
    }
}

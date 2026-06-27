using UnityEngine;
using UnityEngine.InputSystem;
using HammerGolf.RotationSystem;

namespace HammerGolf.Gameplay
{
    public class TimeStopManager : MonoBehaviour
    {
        [Header("Inputs")]
        public InputActionAsset inputActions;
        private InputAction timeStopAction;
        private InputAction rotateAction;
        private InputAction restartAction;

        public Material highlightMaterial;

        [Header("Settings")]
        public float rotationSpeed = 90f;

        private bool isFrozen = false;
        private Rigidbody ballRigidbody;
        private Vector3 savedVelocity;
        private Vector3 savedAngularVelocity;
        private bool wasKinematic;

        private Material[] originalMaterials;
        private Renderer highlightedRenderer;

        private void Awake()
        {
            if (inputActions != null)
            {
                var playerMap = inputActions.FindActionMap("Player");
                if (playerMap != null)
                {
                    timeStopAction = playerMap.FindAction("TimeStop");
                    rotateAction = playerMap.FindAction("RotateSelected");
                    restartAction = playerMap.FindAction("Restart");
                }
            }
        }

        private void OnEnable()
        {
            if (timeStopAction != null)
            {
                timeStopAction.Enable();
                timeStopAction.performed += OnTimeStopToggle;
            }
            if (rotateAction != null)
            {
                rotateAction.Enable();
            }
            if (restartAction != null)
            {
                restartAction.Enable();
                restartAction.performed += OnRestart;
            }
        }

        private void OnDisable()
        {
            if (timeStopAction != null)
            {
                timeStopAction.performed -= OnTimeStopToggle;
                timeStopAction.Disable();
            }
            if (rotateAction != null)
            {
                rotateAction.Disable();
            }
            if (restartAction != null)
            {
                restartAction.performed -= OnRestart;
                restartAction.Disable();
            }
        }

        private void Update()
        {
            if (isFrozen)
            {
                // Handle smooth rotation
                if (rotateAction != null && rotateAction.IsPressed())
                {
                    var activeRotatable = RotationController.Instance?.Active;
                    if (activeRotatable != null)
                    {
                        Vector3 delta = activeRotatable.RotationAxis * rotationSpeed * Time.deltaTime;
                        activeRotatable.Rotate(delta);
                    }
                }

                // Handle Object Highlight
                UpdateHighlight();
            }
            else
            {
                RemoveHighlight();
            }
        }

        private void OnTimeStopToggle(InputAction.CallbackContext context)
        {
            isFrozen = !isFrozen;
            var cam = FindObjectOfType<CameraController>();

            if (ballRigidbody == null)
            {
                var ballController = FindObjectOfType<BallThrowController>();
                if (ballController != null)
                {
                    ballRigidbody = ballController.GetComponent<Rigidbody>();
                }
            }

            if (ballRigidbody != null)
            {
                if (isFrozen)
                {
                    savedVelocity = ballRigidbody.linearVelocity;
                    savedAngularVelocity = ballRigidbody.angularVelocity;
                    wasKinematic = ballRigidbody.isKinematic;
                    ballRigidbody.isKinematic = true;
                }
                else
                {
                    ballRigidbody.isKinematic = wasKinematic;
                    if (!wasKinematic)
                    {
                        ballRigidbody.linearVelocity = savedVelocity;
                        ballRigidbody.angularVelocity = savedAngularVelocity;
                    }
                }
            }

            if (isFrozen)
            {
                if (cam != null) cam.SwitchToFreeCam();
            }
            else
            {
                if (cam != null) 
                {
                    if (wasKinematic) cam.SwitchToPlayer();
                    else cam.SwitchToBall();
                }
            }
        }

        private void OnRestart(InputAction.CallbackContext context)
        {
            var ballController = FindObjectOfType<BallThrowController>();
            if (ballController != null)
            {
                ballController.ReturnToLastShotSafe();
            }
        }

        private void UpdateHighlight()
        {
            var activeRotatable = RotationController.Instance?.Active;
            
            if (activeRotatable != null)
            {
                var mb = activeRotatable as MonoBehaviour;
                if (mb != null)
                {
                    var r = mb.GetComponentInChildren<Renderer>();
                    if (r != null && r != highlightedRenderer)
                    {
                        RemoveHighlight();
                        
                        highlightedRenderer = r;
                        originalMaterials = highlightedRenderer.materials;
                        
                        var newMats = new Material[originalMaterials.Length + 1];
                        for (int i = 0; i < originalMaterials.Length; i++)
                            newMats[i] = originalMaterials[i];
                        newMats[originalMaterials.Length] = highlightMaterial;
                        highlightedRenderer.materials = newMats;
                    }
                }
            }
            else
            {
                RemoveHighlight();
            }
        }

        private void RemoveHighlight()
        {
            if (highlightedRenderer != null && originalMaterials != null)
            {
                highlightedRenderer.materials = originalMaterials;
                highlightedRenderer = null;
                originalMaterials = null;
            }
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.Windows;

namespace HammerGolf
{
    public class CameraController : MonoBehaviour
    {
        #region SerializeFields

        [Header("Targets")]
        [SerializeField] Transform player;
        [SerializeField] Rigidbody ball;

        [Header("Player Follow")]
        [SerializeField] Vector3 playerOffset = new(0f, 5f, -8f);

        [Header("Ball Follow")]
        [SerializeField] float ballDistance = 10f;
        [SerializeField] float ballHeight = 4f;

        [Header("Smoothing")]
        [SerializeField] float positionSmoothSpeed = 8f;

        [Header("Orbit")]
        [SerializeField] float distance = 8f;
        [SerializeField] float heightOffset = 2f;
        [SerializeField] float mouseSensitivity = 150f;
        [SerializeField] float minPitch = -20f;
        [SerializeField] float maxPitch = 70f;

        #endregion

        #region Cache

        CameraMode currentMode = CameraMode.FollowPlayer;
        Transform currentTarget;
        float mouseYaw = 0f;
        float mousePitch = 0f;
        CameraEventsController _camEvents => CameraEventsController.Instance;
        CharacterInputActions input => InputManager.Instance.CharInputActions;


        #endregion

        private void Start()
        {
            currentTarget = player;

            CameraEventsController.SwitchToBall += OnSwitchToBall;
            CameraEventsController.SwitchToPlayer += OnSwitchToPlayer;
        }

        private void OnDestroy()
        {
            CameraEventsController.SwitchToBall -= OnSwitchToBall;
            CameraEventsController.SwitchToPlayer -= OnSwitchToPlayer;
        }

        private void OnEnable()
        {
            transform.LookAt(currentTarget);
        }

        private void OnDisable()
        {
        }

        private void OnSwitchToBall(object sender, EventArgs e)
        {
            SwitchToBall();
        }

        private void OnSwitchToPlayer(object sender, EventArgs e)
        {
            SwitchToPlayer();
        }

        public void SwitchToBall()
        {
            currentMode = CameraMode.FollowBall;
            currentTarget = ball.transform;

        }

        public void SwitchToPlayer()
        {
            currentMode = CameraMode.FollowPlayer;
            currentTarget = player;
        }

        private void LateUpdate()
        {
            OrbitTarget();
        }

        private void OrbitTarget()
        {
            if (currentTarget == null)
                return;

            Vector2 lookInput =
                input.Gameplay.Look.ReadValue<Vector2>();

            mouseYaw +=
                lookInput.x * mouseSensitivity * Time.deltaTime;

            mousePitch -=
                lookInput.y * mouseSensitivity * Time.deltaTime;

            mousePitch = Mathf.Clamp(
                mousePitch,
                minPitch,
                maxPitch);

            Vector3 pivot =
                currentTarget.position +
                Vector3.up * heightOffset;

            Quaternion rotation =
                Quaternion.Euler(
                    mousePitch,
                    mouseYaw,
                    0f);

            Vector3 desiredPosition =
                pivot +
                rotation * new Vector3(
                    0f,
                    0f,
                    -distance);

            transform.position =
                Vector3.Lerp(
                    transform.position,
                    desiredPosition,
                    positionSmoothSpeed * Time.deltaTime);

            transform.LookAt(pivot);
        }
    } 
}
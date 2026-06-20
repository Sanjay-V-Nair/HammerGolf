using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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

    CameraMode currentMode = CameraMode.FollowPlayer;

    CameraEventsController _camEvents => CameraEventsController.Instance;

    private void Start()
    {
        CameraEventsController.SwitchToBall += OnSwitchToBall;
        CameraEventsController.SwitchToPlayer += OnSwitchToPlayer;
    }

    private void OnDestroy()
    {
        CameraEventsController.SwitchToBall -= OnSwitchToBall;
        CameraEventsController.SwitchToPlayer -= OnSwitchToPlayer;
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
    }

    public void SwitchToPlayer()
    {
        currentMode = CameraMode.FollowPlayer;
    }

    private void LateUpdate()
    {
        switch (currentMode)
        {
            case CameraMode.FollowPlayer:
                UpdatePlayerCamera();
                break;

            case CameraMode.FollowBall:
                UpdateBallCamera();
                break;
        }
    }

    private void UpdatePlayerCamera()
    {
        Vector3 targetPosition =
            player.position + playerOffset;

        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                positionSmoothSpeed * Time.deltaTime);

        transform.LookAt(player);
    }

    private void UpdateBallCamera()
    {
        Vector3 velocity =
            ball.linearVelocity;

        Vector3 direction =
            velocity.sqrMagnitude > 0.1f
                ? velocity.normalized
                : ball.transform.forward;

        Vector3 targetPosition =
            ball.position
            - direction * ballDistance
            + Vector3.up * ballHeight;

        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                positionSmoothSpeed * Time.deltaTime);

        transform.LookAt(ball.position);
    }
}
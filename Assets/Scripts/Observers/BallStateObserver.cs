using UnityEngine;

public class BallStateObserver : MonoBehaviour
{
    #region Serialize Feidls
    [Header("References")]
    [SerializeField] BallThrowController ballController;

    [Header("Tuning")]
    [SerializeField] float restVelocityThreshold = 0.15f;
    [SerializeField] float restConfirmTime = 0.4f;

    #endregion

    #region Events

    public event System.Action<Vector3> OnBallAtRest;

    #endregion

    #region Cache

    Rigidbody trackedBall;
    float restTimer;
    private bool isTracking = false;
    CameraEventsController _camEvents => CameraEventsController.Instance;

    #endregion

    void Awake() {
        if (ballController == null) {
            return;
        }

        ballController.OnThrown += BeginTracking;
    }

    void OnDestroy() {
        if (ballController == null) return;
        ballController.OnThrown -= BeginTracking;
    }

    void BeginTracking(Rigidbody ball) {
        trackedBall = ball;
        restTimer = 0f;
        isTracking = true;
    }

    void FixedUpdate() {
        if (!isTracking) return;

        if (trackedBall.linearVelocity.magnitude < restVelocityThreshold) {
            restTimer += Time.fixedDeltaTime;
            if (restTimer >= restConfirmTime) {
                enabled = false;
                OnBallAtRest?.Invoke(trackedBall.position);
                CameraEventsController.SwitchToPlayer?.Invoke(this, null);
            }
        } else {
            restTimer = 0f;
        }
    }

}

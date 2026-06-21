using UnityEngine;

namespace HammerGolf
{
    public class CharacterRecoveryController : MonoBehaviour
    {
        #region Serialize Fields

        public BallStateObserver ballObserver;
        public BallThrowController ballController;

        #endregion

        #region Events

        public event System.Action RecoveryComplete;

        #endregion

        #region Cache

        Transform player;

        #endregion

        void Awake()
        {
            if (player == null) player = transform;

            if (ballObserver == null || ballController == null)
            {
                return;
            }

            ballObserver.OnBallAtRest += HandleBallAtRest;
        }

        void OnDestroy()
        {
            if (ballObserver == null) return;
            ballObserver.OnBallAtRest -= HandleBallAtRest;
        }

        void HandleBallAtRest(Vector3 ballRestPosition)
        {
            player.position = new Vector3(ballRestPosition.x, player.position.y, ballRestPosition.z);
            ballController.ResetToOrbit();
            RecoveryComplete?.Invoke();
        }

    } 
}

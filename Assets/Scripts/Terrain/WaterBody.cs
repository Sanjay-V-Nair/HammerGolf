using UnityEngine;

namespace HammerGolf
{
    public class WaterBody : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            BallThrowController ball =
                other.GetComponent<BallThrowController>();

            if (ball == null)
                return;

            ball.ReturnToLastShot();
        }
    }
}

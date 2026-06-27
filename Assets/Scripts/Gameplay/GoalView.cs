using UnityEngine;

namespace HammerGolf.Gameplay
{
    public class GoalView : MonoBehaviour
    {
        [Tooltip("The collider that detects the ball. If empty, it will auto-assign on Awake.")]
        public Collider goalCollider;

        /// <summary>
        /// Fired when the ball enters the goal.
        /// </summary>
        public static event System.Action OnGameWon;

        private bool hasTriggered = false;

        private void Awake()
        {
            if (goalCollider == null)
                goalCollider = GetComponentInChildren<Collider>();

            if (goalCollider != null)
                goalCollider.isTrigger = true; // Ensure it acts as a trigger area
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;
            
            // Check if it's the ball
            var ballController = other.GetComponentInParent<BallThrowController>();
            if (ballController == null && other.attachedRigidbody != null)
            {
                ballController = other.attachedRigidbody.GetComponent<BallThrowController>();
            }

            if (ballController != null)
            {
                hasTriggered = true;
                
                // Stop and disable the ball
                if (other.attachedRigidbody != null)
                {
                    other.attachedRigidbody.linearVelocity = Vector3.zero;
                    other.attachedRigidbody.angularVelocity = Vector3.zero;
                }
                
                ballController.gameObject.SetActive(false);
                
                OnGameWon?.Invoke();
                Debug.Log("[GoalView] Goal reached! Game Won!");
            }
        }
    }
}

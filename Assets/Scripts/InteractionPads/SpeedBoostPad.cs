using UnityEngine;

namespace HammerGolf
{
    public class SpeedBoostPad : BaseInteractionPad
    {
        [SerializeField] float boostSpeed = 25f;

        protected override void ApplyEffect(Rigidbody ball, Collider other)
        {
            Debug.Log($"Boosting ball with speed: {boostSpeed}");
            ball.AddForce(transform.forward * boostSpeed, ForceMode.Impulse);
        }
    }
}

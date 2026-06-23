using UnityEngine;

namespace HammerGolf
{
    public class SpringPad : BaseInteractionPad
    {
        [SerializeField] float springForce = 30f;
        [SerializeField] Rigidbody rb;

        protected override void ApplyEffect(Rigidbody ball, Collider other)
        {
            var normal = rb.transform.up;
            ball.AddForce(normal * springForce, ForceMode.Impulse);
        }
    }
}

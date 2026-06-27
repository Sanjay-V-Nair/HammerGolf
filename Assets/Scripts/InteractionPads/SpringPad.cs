using UnityEngine;

namespace HammerGolf
{
    public class SpringPad : BaseInteractionPad
    {
        [SerializeField] float springForce = 30f;

        protected override void ApplyEffect(Rigidbody ball, Collider other)
        {
            var normal = transform.up;
            ball.AddForce(normal * springForce, ForceMode.Impulse);
        }
    }
}

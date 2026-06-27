using System.Collections;
using UnityEngine;

namespace HammerGolf
{
    public abstract class BaseInteractionPad : MonoBehaviour
    {
        private bool _isOnCooldown = false;

        protected virtual float CooldownDuration => 1f;

        protected virtual void OnTriggerEnter(Collider other)
        {
            var rb = other.GetComponent<Rigidbody>();

            if (!rb)
                return;

            if (_isOnCooldown)
                return;

            ApplyEffect(rb, other);
            StartCoroutine(InteractionCooldown());
        }

        protected abstract void ApplyEffect(Rigidbody ball, Collider other);
        protected virtual IEnumerator InteractionCooldown()
        {
            _isOnCooldown = true;
            yield return new WaitForSeconds(CooldownDuration);
            _isOnCooldown = false;
        }
    }
}

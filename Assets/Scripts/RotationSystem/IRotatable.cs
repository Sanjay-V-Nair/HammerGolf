using UnityEngine;

namespace HammerGolf.RotationSystem {

    /// <summary>
    /// Implement this on any MonoBehaviour that should be rotatable by the player.
    /// The RotationController will discover and drive these via RotatableRegistrar.
    /// </summary>
    public interface IRotatable {

        /// <summary>Units per second the object rotates.</summary>
        float RotationSpeed { get; }

        /// <summary>
        /// Axes on which this object is allowed to rotate (e.g. Vector3.up for Y-axis only).
        /// The controller will mask its input delta against this before calling Rotate().
        /// </summary>
        Vector3 RotationAxis { get; }

        /// <summary>
        /// Called every frame while this object is the active rotatable.
        /// <paramref name="delta"/> is already axis-masked and speed-scaled by the controller.
        /// </summary>
        void Rotate(Vector3 delta);

        /// <summary>Invoked by the controller when the player selects this object.</summary>
        void OnRotationStart();

        /// <summary>Invoked by the controller when the player deselects this object.</summary>
        void OnRotationEnd();
    }
}
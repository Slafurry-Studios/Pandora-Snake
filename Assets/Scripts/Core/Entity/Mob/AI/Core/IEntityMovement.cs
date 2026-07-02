using UnityEngine;

namespace Game.AI
{
    /// <summary>
    /// Common movement contract implemented by any entity's movement handler
    /// (e.g. NPCMovement for humanoids, CarMovement for vehicles).
    /// EntityBrain and EntityState only ever talk to this interface, so any
    /// movement style can be plugged into the same state machine without
    /// changing Core or rewriting existing states.
    /// </summary>
    public interface IEntityMovement
    {
        /// <summary>
        /// Attempts to move the entity toward desiredDirection at the given speed.
        /// Implementations decide HOW that becomes physical movement
        /// (free 2D velocity for humanoids, steer + throttle for cars, etc).
        /// </summary>
        void SetMovement(Vector2 desiredDirection, float speed);

        /// <summary>
        /// Orients the entity toward a direction without necessarily moving forward.
        /// </summary>
        void FaceDirection(Vector2 direction);
    }
}

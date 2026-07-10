using UnityEngine;

namespace Game.Entities
{
    public interface IEntityMovement
    {
        void Initialize(EntityBrain brain, SpriteRenderer spriteRenderer, Rigidbody2D rb);
        void Initialize(EntityBrain brain, EntityData entityData, SpriteRenderer spriteRenderer, Rigidbody2D rb);
        
        void SetMovement(Vector2 desiredDirection, float speed);
        void FaceDirection(Vector2 direction);
    }
}

using Slafurry.System.Audio;
using UnityEngine;

namespace Game.Entities
{
    public class PanicState : EntityState
    {
        [Header("Panic Thresholds")]
        [Tooltip("How close the snek must be to trigger panic")]
        public float panicRadius = 5f; 
        
        [Tooltip("How far away the snek must be before the NPC feels safe again")]
        public float safeRadius = 8f;  
        
        [Header("Movement")]
        public float speedMultiplier = 5f;

        [Header("Audio")]
        public string soundCategory;
        public string panicSoundName;

        private bool isPanicking = false; 

        public override bool CheckConditions(EntityBrain brain)
        {
            if (brain.Target == null) return false;
            
            float distance = Vector2.Distance(transform.position, brain.Target.position);

            if (isPanicking)
            {
                return distance <= safeRadius;
            }

            else
            {
                return distance <= panicRadius;
            }
        }

        public override void EnterState(EntityBrain brain) 
        { 
            isPanicking = true;
            Audio.PlaySFX2D(soundCategory,panicSoundName);
        }

        public override void UpdateState(EntityBrain brain)
        {
            float runSpeed = entityData.MoveSpeed * speedMultiplier;

            Vector2 runDirection = ((Vector2)transform.position - (Vector2)brain.Target.position).normalized;
            brain.EntityMovement.SetMovement(runDirection, runSpeed);
        }

        public override void ExitState(EntityBrain brain)
        {
            isPanicking = false;
            brain.EntityMovement.SetMovement(Vector2.zero, 0f);
        }
    }
}
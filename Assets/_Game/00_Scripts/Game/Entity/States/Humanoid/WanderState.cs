using UnityEngine;

namespace Game.AI
{
    public class WanderState : EntityState
    {
        [Header("Wander Settings")]
        public float speedMultiplier = 2f;
        public float minWanderTime = 1f;
        public float maxWanderTime = 4f;

        private float stateTimer;

        public override bool CheckConditions(EntityBrain brain)
        {
            return true; 
        }

        public override void EnterState(EntityBrain brain)
        {
            PickNewWanderDirection(brain);
        }

        public override void UpdateState(EntityBrain brain)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f) PickNewWanderDirection(brain);
        }

        public override void ExitState(EntityBrain brain)
        {
            brain.Movement.SetMovement(Vector2.zero, 0f);
        }

        private void PickNewWanderDirection(EntityBrain brain)
        {
            var player = brain.Target.GetComponent<Game.Player.PlayerMovement>();

            float playerSpeed = (player != null) ? player.CurrentSpeed : 5f;

            float walkSpeed = playerSpeed * speedMultiplier; 

            Vector2 moveDirection = Random.value > 0.5f ? Random.insideUnitCircle.normalized : Vector2.zero;
            brain.Movement.SetMovement(moveDirection, walkSpeed);
            stateTimer = Random.Range(minWanderTime, maxWanderTime);
        }
    }
}
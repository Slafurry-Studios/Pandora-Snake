using UnityEngine;

namespace Game.AI
{
    /// <summary>
    /// Non-hostile "traffic" driving: the car picks a random heading periodically and
    /// keeps steering toward it (CarMovement handles the actual turning curve).
    /// Use this as the ONLY state for civilian/non-hostile cars, or as the lowest
    /// priority fallback state on a hostile car's stateList.
    /// </summary>
    public class CarWanderState : EntityState
    {
        [Header("Wander Settings")]
        public float speedMultiplier = 1.5f;
        public float minWanderTime = 2f;
        public float maxWanderTime = 5f;

        private Vector2 wanderDirection;
        private float wanderSpeed;
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

            brain.Movement.SetMovement(wanderDirection, wanderSpeed);
        }

        public override void ExitState(EntityBrain brain)
        {
            brain.Movement.SetMovement(Vector2.zero, 0f);
        }

        private void PickNewWanderDirection(EntityBrain brain)
        {
            var player = brain.Target != null ? brain.Target.GetComponent<Game.Player.PlayerMovement>() : null;
            float playerSpeed = (player != null) ? player.CurrentSpeed : 5f;

            wanderSpeed = playerSpeed * speedMultiplier;
            wanderDirection = Random.insideUnitCircle.normalized;
            stateTimer = Random.Range(minWanderTime, maxWanderTime);
        }
    }
}

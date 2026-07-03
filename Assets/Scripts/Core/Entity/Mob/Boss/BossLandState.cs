using UnityEngine;
using Game.AI;

namespace Game.AI.Boss
{
    public class BossLandState : EntityState
    {
        [Header("Landing Thresholds")]
        [Tooltip("If boss HP falls below this percentage (0 to 1), it attempts to retreat and land. Default: 0.5 (50%).")]
        [Range(0f, 1f)]
        public float landingHpThresholdPct = 0.5f;

        [Tooltip("Duration (in seconds) the boss remains landed on the support building receiving buffs/repairs. Default: 2s.")]
        public float landingDuration = 2f;

        [Tooltip("Cooldown time (in seconds) between landing attempts. Default: 10s.")]
        public float landingCooldown = 10f;

        [Header("Movement & Precision")]
        [Tooltip("Distance threshold (in units) to touch down on the support building. Default: 1.5 units.")]
        public float landingRadius = 1.5f;

        [Tooltip("Speed multiplier relative to player speed when retreating toward the support building. Default: 1.5x.")]
        public float retreatSpeedMultiplier = 1.5f;

        [Header("Rotation")]
        [Tooltip("Degrees per second the boss rotates to face its movement direction while retreating. Default: 180.")]
        public float rotationSpeed = 180f;

        [Tooltip("Angle offset (degrees) to correct for the sprite's default facing direction. Default: 90 (sprite faces down at rotation 0).")]
        public float spriteFacingOffset = 90f;

        private bool isLanded = false;
        private float landEndTime;
        private float nextLandTime;
        private bool isExecutingLanding = false;

        public override bool CheckConditions(EntityBrain brain)
        {
            BossHealth health = brain.GetComponent<BossHealth>();
            if (health == null || health.IsDead) return false;

            if (isExecutingLanding) return true;

            if (health.ActiveSupportBuildings == null || health.ActiveSupportBuildings.Count == 0) return false;

            float hpPct = health.CurrentHealth / health.MaxHealth;

            if (hpPct >= landingHpThresholdPct) return false;
            if (Time.time < nextLandTime) return false;

            return true;
        }

        public override void EnterState(EntityBrain brain)
        {
            isExecutingLanding = true;
            isLanded = false;
            Debug.Log($"[BossLandState] HP below threshold ({landingHpThresholdPct * 100}%)! Initiating retreat to land on Support Building.");
        }

        public override void UpdateState(EntityBrain brain)
        {
            BossHealth health = brain.GetComponent<BossHealth>();
            if (health == null || health.IsDead || health.ActiveSupportBuildings.Count == 0)
            {
                FinishLandingRoutine(brain);
                return;
            }

            BossSupportBuilding targetBuilding = GetClosestSupportBuilding(health);
            if (targetBuilding == null)
            {
                FinishLandingRoutine(brain);
                return;
            }

            float distance = Vector2.Distance(transform.position, targetBuilding.transform.position);

            if (!isLanded)
            {
                if (distance <= landingRadius)
                {
                    isLanded = true;
                    if (brain.Movement != null) brain.Movement.SetMovement(Vector2.zero, 0f);

                    landEndTime = Time.time + landingDuration;
                    Debug.Log($"[BossLandState] Landed on {targetBuilding.name} for {landingDuration} seconds!");
                }
                else
                {
                    Vector2 direction = (targetBuilding.transform.position - transform.position).normalized;
                    float baseSpeed = (brain.Target != null && brain.Target.GetComponent<Game.Player.PlayerMovement>() != null)
                        ? brain.Target.GetComponent<Game.Player.PlayerMovement>().CurrentSpeed
                        : 5f;
                    float speed = baseSpeed * retreatSpeedMultiplier;

                    if (brain.Movement != null) brain.Movement.SetMovement(direction, speed);
                    RotateTowards(direction);
                }
            }
            else
            {
                if (brain.Movement != null) brain.Movement.SetMovement(Vector2.zero, 0f);

                if (Time.time >= landEndTime)
                {
                    Debug.Log($"[BossLandState] Completed landing duration. Resuming combat!");
                    FinishLandingRoutine(brain);
                }
            }
        }

        private void RotateTowards(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spriteFacingOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private BossSupportBuilding GetClosestSupportBuilding(BossHealth health)
        {
            BossSupportBuilding closest = null;
            float minDist = float.MaxValue;

            foreach (var b in health.ActiveSupportBuildings)
            {
                if (b == null) continue;
                float d = Vector2.Distance(transform.position, b.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    closest = b;
                }
            }
            return closest;
        }

        private void FinishLandingRoutine(EntityBrain brain)
        {
            isExecutingLanding = false;
            isLanded = false;
            nextLandTime = Time.time + landingCooldown;
        }

        public override void ExitState(EntityBrain brain)
        {
            isExecutingLanding = false;
            isLanded = false;
            if (brain.Movement != null)
            {
                brain.Movement.SetMovement(Vector2.zero, 0f);
            }
        }
    }
}
using Slafurry.Utils.Attributes;
using UnityEngine;

namespace Game.Entities
{
    [GameAssetCreator("Entity/Non Hostile", "Car", order: 5)]
    public class CarData : EntityData
    {
        [Header("Forward Driving")]
        [SerializeField] private float turnSpeed;
        [SerializeField] private float turnSpeedPenalty;
        [SerializeField] private float brakeDeceleration;

        [Header("Reverse Driving")]
        [SerializeField] private float reverseAngleThreshold;
        [SerializeField][Range(0f, 1f)] private float reverseSpeedMultiplier;

        public float TurnSpeed => turnSpeed;
        public float TurnSpeedPenalty => turnSpeedPenalty;
        public float BrakeDeceleration => brakeDeceleration;
        public float ReverseAngleThreshold => reverseAngleThreshold;
        public float ReverseSpeedMultiplier => reverseSpeedMultiplier;
    }
}
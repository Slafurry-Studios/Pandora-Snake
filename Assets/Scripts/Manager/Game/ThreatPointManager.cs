using UnityEngine;
using Game.Utils;
using System;

namespace Game.Manager
{

    public class ThreatPointManager : MonoBehaviour
    {
        [SerializeField] private int MaxPoint = 5000;
        [SerializeField] private Threshold[] threatThresholds;
        [SerializeField] private BaseObjectiveChannel[] threatPointChannel;
        public event Action<float, float> OnThreatPointIncreased;
        public event Action<int> OnCurrentThreatStateChanged;
        [SerializeField] private ObjectiveScriptableObject Objective;



        [SerializeField] private int threatPoints = 0;
        [SerializeField] private int currentThreatState = 0;

        public void IncreasePoints(int amount)
        {
            Debug.Log($"[ThreatPointManager] IncreasePoints called with {amount}");
            threatPoints += amount;

            int newThreshold = GetCurrentThreshold();

            OnThreatPointIncreased?.Invoke(threatPoints, MaxPoint);

            foreach (BaseObjectiveChannel channel in threatPointChannel)
            {
                channel.Raise(amount);
            }

            if (newThreshold > currentThreatState)
            {
                currentThreatState = newThreshold;
                Debug.Log($"[ThreatPointManager] Invoking OnCurrentThreatStateChanged with {currentThreatState}. Subscriber count: {OnCurrentThreatStateChanged?.GetInvocationList().Length ?? 0}");
                OnCurrentThreatStateChanged?.Invoke(currentThreatState);
            }

            if (currentThreatState > 0)
            {
                ObjectiveManager.Instance.AddObjective(Objective.Objective);
            }
        }

        private int GetCurrentThreshold()
        {
            int level = 0;

            for (int i = 0; i < threatThresholds.Length; i++)
            {
                if (threatPoints >= threatThresholds[i].ThresholdValue)
                {
                    level = i + 1;   // <-- ubah ini
                }
            }

            return level;
        }

        public int GetPoint()
        {
            return threatPoints;
        }
    }
}
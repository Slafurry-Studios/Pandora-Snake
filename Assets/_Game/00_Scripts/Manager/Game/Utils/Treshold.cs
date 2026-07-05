using UnityEngine;


namespace Game.Utils
{

    [System.Serializable]
    public struct Threshold
    {
        public string Name;
        public int Level;
        public int ThresholdValue;
        public Threshold(string name, int level, int thresholdValue)
        {
            Name = name;
            Level = level;
            ThresholdValue = thresholdValue;
        }
    }
}
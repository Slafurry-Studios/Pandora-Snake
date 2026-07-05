using UnityEngine;
using UnityEngine.UI;

namespace Game.Upgrade
{

    [CreateAssetMenu(fileName = "NewUpgradeCard", menuName = "Game/Upgrade/Card")]
    public class UpgradeCard : ScriptableObject
    {
        public UpgradeCardData UpgradeCardData;
        public UpgradeCard[] Prequesities;
        public UpgradeCardEffect effect;
        public void OnSelected()
        {
            effect.Apply();
        }
    }
    [System.Serializable]
    public struct UpgradeCardData
    {
        public Sprite UpgradeBg;
        
        [Tooltip("Drop Weight")]
        public float Weight;
    }
}
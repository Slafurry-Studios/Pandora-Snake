using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/UrbanChaosChannel")]
public class UrbanChaosChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(500);
    }
}

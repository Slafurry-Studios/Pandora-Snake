using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/UrbanChaosChannel")]
public class UrbanChaosChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(500);
    }
}

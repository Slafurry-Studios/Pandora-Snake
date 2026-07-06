using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/TankHunterChannel")]
public class TankHunterChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(1000);
    }
}

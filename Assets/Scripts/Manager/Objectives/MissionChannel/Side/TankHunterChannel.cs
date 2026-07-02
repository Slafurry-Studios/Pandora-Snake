using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/TankHunterChannel")]
public class TankHunterChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(1000);
    }
}

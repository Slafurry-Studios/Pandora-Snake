using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/SpeedFreakChannel")]
public class SpeedFreakChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(200);
    }
}

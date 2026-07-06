using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/TicketVictimChannel")]
public class TicketVictimChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(500);
    }
}

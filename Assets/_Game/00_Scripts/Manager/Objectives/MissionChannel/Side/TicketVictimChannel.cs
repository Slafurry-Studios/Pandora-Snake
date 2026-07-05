using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/TicketVictimChannel")]
public class TicketVictimChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(500);
    }
}

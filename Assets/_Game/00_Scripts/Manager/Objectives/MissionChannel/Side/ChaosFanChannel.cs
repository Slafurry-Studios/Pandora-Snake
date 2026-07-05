using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/ChaosFanChannel")]
public class ChaosFanChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(200);
    }
}

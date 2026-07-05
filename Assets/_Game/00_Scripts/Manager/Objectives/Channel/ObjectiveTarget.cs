using UnityEngine;

public class ObjectiveTarget : MonoBehaviour
{
    [SerializeField] private BaseObjectiveChannel channel;

    public void SetChannel(BaseObjectiveChannel newChannel)
    {
        channel = newChannel;
    }

    private void OnEnable()
    {
        if (channel != null)
            ObjectiveTargetRegistry.Register(channel, transform);
    }

    private void OnDisable()
    {
        if (channel != null)
            ObjectiveTargetRegistry.Unregister(channel, transform);
    }
}
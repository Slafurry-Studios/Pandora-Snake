public class ObjectiveProgress
{
    public Objective Data;
    public float CurrentValue;

    public bool IsCompleted => CurrentValue >= Data.ObjectiveThreshold;

    public ObjectiveProgress(Objective data)
    {
        Data = data;
        CurrentValue = 0f;
    }
}
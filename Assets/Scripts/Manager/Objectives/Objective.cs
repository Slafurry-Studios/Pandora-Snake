using UnityEngine;

[System.Serializable]
public struct Objective : System.IEquatable<Objective>
{
    public string DisplayName;
    public BaseObjectiveChannel Channel;
    public float ObjectiveThreshold;
    public ObjectiveType ObjectiveType;
    public bool IsMainMission;

    public bool Equals(Objective other)
    {
        return Channel == other.Channel;
    }

    public override bool Equals(object obj)
    {
        return obj is Objective other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Channel != null ? Channel.GetHashCode() : 0;
    }
}
// ObjectiveTargetRegistry.cs
using System.Collections.Generic;
using UnityEngine;

public static class ObjectiveTargetRegistry
{
    private static readonly Dictionary<BaseObjectiveChannel, List<Transform>> targets = new();

    public static void Register(BaseObjectiveChannel channel, Transform target)
    {
        if (!targets.ContainsKey(channel))
            targets[channel] = new List<Transform>();

        if (!targets[channel].Contains(target))
            targets[channel].Add(target);
    }

    public static void Unregister(BaseObjectiveChannel channel, Transform target)
    {
        if (targets.TryGetValue(channel, out var list))
            list.Remove(target);
    }

    public static Transform GetNearest(BaseObjectiveChannel channel, Vector3 fromPosition)
    {
        if (!targets.TryGetValue(channel, out var list) || list.Count == 0)
            return null;

        Transform nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var t in list)
        {
            if (t == null) continue;
            float dist = Vector3.Distance(fromPosition, t.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = t;
            }
        }

        return nearest;
    }

    public static int CountAvailable(BaseObjectiveChannel channel)
    {
        if (!targets.TryGetValue(channel, out var list)) return 0;
        list.RemoveAll(t => t == null);
        return list.Count;
    }
}
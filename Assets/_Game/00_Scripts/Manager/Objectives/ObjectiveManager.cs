using UnityEngine;
using System.Collections.Generic;

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    private readonly List<Objective> activeObjectives = new();
    private readonly HashSet<Objective> completedObjectives = new(); // ← baru
    private readonly Dictionary<Objective, float> progress = new();
    private readonly Dictionary<Objective, System.Action<float>> handlers = new();

    public event System.Action<Objective, float> OnObjectiveProgress;
    public event System.Action<Objective> OnObjectiveCompleted;
    public event System.Action<Objective> OnObjectiveAdded;

    public void AddObjective(Objective objective)
    {
        if (objective.Channel == null)
        {
            Debug.LogWarning($"Objective '{objective.DisplayName}' not had any channel, skipping.");
            return;
        }

        if (completedObjectives.Contains(objective)) // ← block re-add
        {
            // Debug.LogWarning($"Objective '{objective.DisplayName}' already completed, skipping.");
            return;
        }

        if (activeObjectives.Contains(objective))
        {
            // Debug.LogWarning($"Objective '{objective.DisplayName}' already active, skipping.");
            return;
        }

        activeObjectives.Add(objective);
        progress[objective] = 0f;

        System.Action<float> handler = amount => HandleProgress(objective, amount);
        handlers[objective] = handler;
        objective.Channel.OnRaised += handler;

        OnObjectiveAdded?.Invoke(objective);
    }

    public void RemoveObjective(Objective objective)
    {
        if (!activeObjectives.Contains(objective))
            return;

        if (handlers.TryGetValue(objective, out var handler))
        {
            objective.Channel.OnRaised -= handler;
            handlers.Remove(objective);
        }

        activeObjectives.Remove(objective);
        progress.Remove(objective);
    }

    private void HandleProgress(Objective objective, float amount)
    {
        if (!progress.ContainsKey(objective))
            return;
        progress[objective] += amount;
        OnObjectiveProgress?.Invoke(objective, progress[objective]);

        if (progress[objective] >= objective.ObjectiveThreshold)
        {
            CompleteObjective(objective);
        }
    }

    private void CompleteObjective(Objective objective)
    {
        completedObjectives.Add(objective); // ← tandai completed permanen
        OnObjectiveCompleted?.Invoke(objective);
        objective.Channel.OnCompleted();
        RemoveObjective(objective);
    }

    public float GetProgress(Objective objective)
    {
        return progress.TryGetValue(objective, out var value) ? value : 0f;
    }

    public bool IsActive(Objective objective)
    {
        return activeObjectives.Contains(objective);
    }

    public bool IsCompleted(Objective objective) // ← opsional, berguna buat query dari luar
    {
        return completedObjectives.Contains(objective);
    }

    private void OnDisable()
    {
        foreach (var kvp in handlers)
        {
            kvp.Key.Channel.OnRaised -= kvp.Value;
        }
        handlers.Clear();
        progress.Clear();
        activeObjectives.Clear();
        // completedObjectives sengaja TIDAK di-clear, biar tetap keinget kalau OnDisable/OnEnable siklus
        // (kalau memang mau reset total pas scene reload, itu urusan terpisah, lihat catatan di bawah)
    }
}
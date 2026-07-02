using UnityEngine;
using Game.Manager;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Spawner[] spawners;

    void Start()
    {
        GameManager.Instance.threatManager.OnCurrentThreatStateChanged += HandleThreatStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.threatManager != null)
        {
            GameManager.Instance.threatManager.OnCurrentThreatStateChanged -= HandleThreatStateChanged;
        }
    }


    private void HandleThreatStateChanged(int newThreatState)
    {
        Debug.Log($"[SpawnManager] Threat state changed to {newThreatState}. Activating spawners with threat state <= {newThreatState}.");
        foreach (var spawner in spawners)
        {
            if (spawner.threatState <= newThreatState)
                spawner.spawner.gameObject.SetActive(true);
        }
    }
}

[System.Serializable]
public struct Spawner
{
    public string spawnerName;
    public int threatState;
    public EntitySpawner spawner;
}
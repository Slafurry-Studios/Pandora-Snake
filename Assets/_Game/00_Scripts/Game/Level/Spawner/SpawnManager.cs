using UnityEngine;
using Game.Managerd;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Spawner[] spawners;
    private bool[] activated;

    void Start()
    {
        activated = new bool[spawners.Length];
        GameManagerOld.Instance.threatManager.OnCurrentThreatStateChanged += HandleThreatStateChanged;
    }

    private void OnDisable()
    {
        if (GameManagerOld.Instance != null &&
            GameManagerOld.Instance.threatManager != null)
        {
            GameManagerOld.Instance.threatManager.OnCurrentThreatStateChanged -= HandleThreatStateChanged;
        }
    }

    private void HandleThreatStateChanged(int newThreatState)
    {
        Debug.Log($"[SpawnManager] Threat state changed to {newThreatState}. Activating spawners with threat state <= {newThreatState}.");
        for (int i = 0; i < spawners.Length; i++)
        {
            var spawner = spawners[i];
            if (!activated[i] && spawner.threatState <= newThreatState)
            {
                spawner.spawner.gameObject.SetActive(true);
                spawner.onSpawnerActive?.Invoke();
                activated[i] = true;
            }
        }
    }
}

[System.Serializable]
public struct Spawner
{
    public string spawnerName;
    public int threatState;
    public EntitySpawner spawner;
    public UnityEvent onSpawnerActive;
}
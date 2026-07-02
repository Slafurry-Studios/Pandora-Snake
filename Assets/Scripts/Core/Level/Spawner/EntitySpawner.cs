using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntitySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] entityPrefabs;

    [Header("Spawn Limit")]
    [SerializeField] private int maxEntities = 10;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int spawnsPerTick = 1;

    [Header("Spawn Area")]
    [SerializeField] private float minSpawnRadius = 6f;
    [SerializeField] private float maxSpawnRadius = 14f;
    [SerializeField] private int maxAttemptsPerSpawn = 10;

    private Generator generator;
    private readonly List<GameObject> spawnedEntities = new List<GameObject>();
    private float timer;

    void Awake()
    {
        generator = FindAnyObjectByType<Generator>();
    }

    void Update()
    {
        if (player == null || entityPrefabs.Length == 0 || generator == null || generator.Grid == null)
            return;

        spawnedEntities.RemoveAll(e => e == null);

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        int toSpawn = Mathf.Min(spawnsPerTick, maxEntities - spawnedEntities.Count);
        for (int i = 0; i < toSpawn; i++)
        {
            TrySpawnOne();
        }
    }

    private void TrySpawnOne()
    {
        Tilemap map = generator.RoadTilemap;

        for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 worldPos = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * dist;

            Vector3Int cell = map.WorldToCell(worldPos);

            if (cell.x < 0 || cell.x >= generator.Width || cell.y < 0 || cell.y >= generator.Height)
                continue;

            if (generator.Grid[cell.x, cell.y] == 2)
                continue;

            Vector3 spawnPos = map.GetCellCenterWorld(cell);
            GameObject prefab = entityPrefabs[Random.Range(0, entityPrefabs.Length)];

            GameObject newEntity = Instantiate(prefab, spawnPos, Quaternion.identity);
            newEntity.transform.parent = transform;
            spawnedEntities.Add(newEntity);
            return;
        }
    }

    public void ClearAll()
    {
        foreach (var e in spawnedEntities)
        {
            if (e != null) Destroy(e);
        }
        spawnedEntities.Clear();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, minSpawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxSpawnRadius);
    }
#endif
}
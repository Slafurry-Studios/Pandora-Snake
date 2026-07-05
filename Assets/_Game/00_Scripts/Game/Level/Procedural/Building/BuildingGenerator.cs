using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour, IGenerate
{
    [SerializeField] private int order = 3;

    [Header("Prefabs & Size")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Vector2Int buildingSize = new Vector2Int(3, 3);

    [Header("Margins (X = Min, Y = Max empty tiles)")]
    [SerializeField] private Vector2Int marginLeftRange = new Vector2Int(1, 3);
    [SerializeField] private Vector2Int marginRightRange = new Vector2Int(1, 3);
    [SerializeField] private Vector2Int marginBottomRange = new Vector2Int(1, 3);
    [Tooltip("0 = can touch top road")]
    [SerializeField] private Vector2Int marginTopRange = new Vector2Int(0, 1);

    private Generator generator;
    private readonly List<GameObject> spawnedBuildings = new List<GameObject>();

    public int Order => order;

    void Awake()
    {
        generator = FindAnyObjectByType<Generator>();
    }

    public void Generate()
    {
        foreach (GameObject b in spawnedBuildings) Destroy(b);
        spawnedBuildings.Clear();

        if (prefabs.Length == 0) return;

        int[,] grid = generator.Grid;
        int width = generator.Width;
        int height = generator.Height;

        // Buat list semua koordinat, lalu acak urutannya
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                positions.Add(new Vector2Int(x, y));

        // Fisher-Yates shuffle
        for (int i = positions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        foreach (var pos in positions)
        {
            int x = pos.x, y = pos.y;

            int mLeft = Random.Range(marginLeftRange.x, marginLeftRange.y + 1);
            int mRight = Random.Range(marginRightRange.x, marginRightRange.y + 1);
            int mBottom = Random.Range(marginBottomRange.x, marginBottomRange.y + 1);
            int mTop = Random.Range(marginTopRange.x, marginTopRange.y + 1);

            if (BuildingUtils.CanFitBuildingWithMargins(grid, x, y, buildingSize.x, buildingSize.y, mLeft, mRight, mBottom, mTop))
            {
                GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];
                Vector3 spawnPos = generator.RoadTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                spawnPos += new Vector3((buildingSize.x - 1) * 0.5f, (buildingSize.y - 1) * 0.5f, 0);

                GameObject newBuilding = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                newBuilding.transform.parent = generator.transform;
                spawnedBuildings.Add(newBuilding);

                BuildingUtils.MarkGridOccupied(grid, x, y, buildingSize.x, buildingSize.y);
            }
        }
    }
}
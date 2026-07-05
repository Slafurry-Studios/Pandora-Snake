using System.Collections.Generic;
using UnityEngine;

public class BossBuildingGenerator : MonoBehaviour, IGenerate
{
    [SerializeField] private int order = 2;

    [Header("Boss Building (Single, Open Plaza)")]
    [Tooltip("Empty tiles required around the boss building (creates a big plaza/open square).")]
    [SerializeField] private Vector2Int bossMarginRange = new Vector2Int(4, 6);
    [SerializeField] private Vector2Int bossSize = new Vector2Int(4, 4);
    [SerializeField] private GameObject[] bossBuildings;

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

        if (bossBuildings.Length == 0) return;

        int[,] grid = generator.Grid;
        int width = generator.Width;
        int height = generator.Height;

        // Collect every valid (x, y) spot first, then pick ONE at random.
        // This avoids bias towards the bottom-left corner that a simple "first found" scan would have.
        List<Vector2Int> validSpots = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int mLeft = Random.Range(bossMarginRange.x, bossMarginRange.y + 1);
                int mRight = Random.Range(bossMarginRange.x, bossMarginRange.y + 1);
                int mBottom = Random.Range(bossMarginRange.x, bossMarginRange.y + 1);
                int mTop = Random.Range(bossMarginRange.x, bossMarginRange.y + 1);

                if (BuildingUtils.CanFitBuildingWithMargins(grid, x, y, bossSize.x, bossSize.y, mLeft, mRight, mBottom, mTop))
                {
                    validSpots.Add(new Vector2Int(x, y));
                }
            }
        }

        if (validSpots.Count == 0) return; // No room for a plaza this layout, just skip the boss.

        Vector2Int spot = validSpots[Random.Range(0, validSpots.Count)];

        GameObject prefabToSpawn = bossBuildings[Random.Range(0, bossBuildings.Length)];
        Vector3 spawnPos = generator.RoadTilemap.GetCellCenterWorld(new Vector3Int(spot.x, spot.y, 0));
        spawnPos += new Vector3((bossSize.x - 1) * 0.5f, (bossSize.y - 1) * 0.5f, 0);

        GameObject newBuilding = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        newBuilding.transform.parent = generator.transform;
        spawnedBuildings.Add(newBuilding);

        // Mark the building footprint AND the plaza margin as occupied (3 = reserved plaza)
        // so other buildings can't spawn inside the open space around the boss.
        int plazaMargin = bossMarginRange.y; // use the max margin to keep the plaza generous
        BuildingUtils.MarkGridReserved(grid, spot.x - plazaMargin, spot.y - plazaMargin,
            bossSize.x + plazaMargin * 2, bossSize.y + plazaMargin * 2);

        // Re-mark the actual footprint as a real building (2) so visuals/logic treat it consistently.
        BuildingUtils.MarkGridOccupied(grid, spot.x, spot.y, bossSize.x, bossSize.y);
    }
}
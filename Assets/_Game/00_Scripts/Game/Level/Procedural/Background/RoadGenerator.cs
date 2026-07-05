using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadGenerator : MonoBehaviour, IGenerate
{
    [Header("Generation Order")]
    [SerializeField] private int order = 1;

    [Header("Road Generation (BSP)")]
    [SerializeField] private TileBase roadRuleTile;
    [Tooltip("The smallest a block can get before roads stop dividing it.")]
    [SerializeField] private int minBlockSize = 8;
    [Tooltip("The maximum size a block can be. If it's larger, it MUST divide.")]
    [SerializeField] private int maxBlockSize = 30;
    [Range(0f, 1f)]
    [SerializeField] private float stopDividingChance = 0.15f;
    [Range(0f, 1f)]
    [SerializeField] private float deadEndChance = 0.2f;

    private Generator generator;

    public int Order => order;

    void Awake()
    {
        generator = FindAnyObjectByType<Generator>();
    }

    public void Generate()
    {
        generator.RoadTilemap.ClearAllTiles();
        GenerateRecursive(0, 0, generator.Width, generator.Height, 0);
    }

    private void GenerateRecursive(int startX, int startY, int width, int height, int depth)
    {
        bool isTooBig = width > maxBlockSize || height > maxBlockSize;

        if (!isTooBig)
        {
            if (width < minBlockSize * 2 || height < minBlockSize * 2) return;
            if (depth > 1 && Random.value < stopDividingChance) return;
        }

        bool splitHorizontal;
        if (width > height * 1.5f) splitHorizontal = false;
        else if (height > width * 1.5f) splitHorizontal = true;
        else splitHorizontal = Random.value > 0.5f;

        // Only allow dead-ends on deeper branches so main city arteries stay connected!
        bool isDeadEnd = depth > 2 && Random.value < deadEndChance;

        if (splitHorizontal)
        {
            int roadY = startY + Random.Range(minBlockSize, height - minBlockSize);
            int drawStartX = startX;
            int drawEndX = startX + width;

            if (isDeadEnd)
            {
                int stopPoint = Random.Range(minBlockSize, width - minBlockSize);
                if (Random.value > 0.5f) drawStartX = startX + stopPoint;
                else drawEndX = startX + stopPoint;
            }

            for (int x = drawStartX; x < drawEndX; x++)
            {
                generator.Grid[x, roadY] = 1;
                generator.RoadTilemap.SetTile(new Vector3Int(x, roadY, 0), roadRuleTile);
            }

            if (!isDeadEnd)
            {
                GenerateRecursive(startX, startY, width, roadY - startY, depth + 1);
                GenerateRecursive(startX, roadY + 1, width, (startY + height) - (roadY + 1), depth + 1);
            }
        }
        else
        {
            int roadX = startX + Random.Range(minBlockSize, width - minBlockSize);
            int drawStartY = startY;
            int drawEndY = startY + height;

            if (isDeadEnd)
            {
                int stopPoint = Random.Range(minBlockSize, height - minBlockSize);
                if (Random.value > 0.5f) drawStartY = startY + stopPoint;
                else drawEndY = startY + stopPoint;
            }

            for (int y = drawStartY; y < drawEndY; y++)
            {
                generator.Grid[roadX, y] = 1;
                generator.RoadTilemap.SetTile(new Vector3Int(roadX, y, 0), roadRuleTile);
            }

            if (!isDeadEnd)
            {
                GenerateRecursive(startX, startY, roadX - startX, height, depth + 1);
                GenerateRecursive(roadX + 1, startY, (startX + width) - (roadX + 1), height, depth + 1);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundGenerator : MonoBehaviour, IGenerate
{
    [SerializeField] private int order = 0;
    [SerializeField] private Tilemap bgTilemap;
    [SerializeField] private TileBase bgRuleTile;

    private Generator generator;

    public int Order => order;

    void Awake()
    {
        generator = FindAnyObjectByType<Generator>();
    }

    public void Generate()
    {
        bgTilemap.ClearAllTiles();

        for (int x = 0; x < generator.Width; x++)
        {
            for (int y = 0; y < generator.Height; y++)
            {
                bgTilemap.SetTile(new Vector3Int(x, y, 0), bgRuleTile);
            }
        }
    }
}
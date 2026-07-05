using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generator : MonoBehaviour
{
    [Header("City Dimensions")]
    [SerializeField] private int width = 60;
    [SerializeField] private int height = 40;

    [Header("Shared Tilemap")]
    [Tooltip("Used by Road, Boss Building, and regular Building generators for world-position conversion.")]
    [SerializeField] private Tilemap roadTilemap;

    public int Width => width;
    public int Height => height;
    public Tilemap RoadTilemap => roadTilemap;

    public int[,] Grid { get; private set; }

    private List<IGenerate> generators;

    void Start()
    {
        GenerateCity();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     GenerateCity();
        // }
    }

    void GenerateCity()
    {
        Grid = new int[width, height];

        if (generators == null)
        {
            generators = GetComponentsInChildren<IGenerate>(true)
                .OrderBy(g => g.Order)
                .ToList();
        }

        foreach (IGenerate generator in generators)
        {
            generator.Generate();
        }
    }
}
﻿using UnityEngine;

/// <summary>
/// Defines the different types of linked meshes we can use
/// </summary>
public enum HexEdgeType
{
    Flat, Slope, Cliff
}

public class HexMetrics : Singleton<HexMetrics>
{
    /// <summary>
    /// Radius that defines the corners of the Hex
    /// </summary>
    [SerializeField]
    [Tooltip("Size of the circle that the corners of the hex will sit on")]
    private float _outerRadius = 1f;
    public float outerRadius { get { return _outerRadius; } }
    public float outerToInner { get { return 0.866025404f; } }
    /// <summary>
    /// Radius that is the extents of the edges
    /// </summary>
    public float innerRadius { get { return outerRadius * outerToInner; } }
    public float innerToOuter { get { return 1f / outerToInner; } }

    /// <summary>
    /// Fill percent of the Hex before the bleed to the next cell starts
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("How much of the hex will be solid color")]
    private float _solidFactor = 0.8f;
    public float solidFactor { get { return _solidFactor; } }
    /// <summary>
    /// Leftover percent of the solid factor which is used for the distance that we need to bleed
    /// </summary>
    public float blendFactor { get { return 1f - solidFactor; } }

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("How much of a water hex will be as the center mesh")]
    private float _waterFactor = 0.6f;
    public float waterFactor { get { return _waterFactor; } }

    public float waterBlendFactor { get { return 1f - waterFactor; } }

    /// <summary>
    /// This defines how large the distance between the tops of the hexes are
    /// </summary>
    [SerializeField]
    [Tooltip("How high should each successive elevation step be")]
    private float _elevationStep = 3f;
    public float elevationStep { get { return _elevationStep; } }

    /// <summary>
    /// How many terraces will be made when the conditions to generate them happen
    /// </summary>
    [SerializeField]
    [Tooltip("The amount of steps for the terrace bridge")]
    private int _terracesPerSlope = 2;
    public int terracesPerSlope { get { return _terracesPerSlope; } }
    /// <summary>
    /// The actual value we use during generation
    /// </summary>
    public int terraceSteps { get { return terracesPerSlope * 2 + 1; } }
    /// <summary>
    /// The percent of how wide each terrace will be
    /// </summary>
    public float horizontalTerraceStepSize { get { return 1f / terraceSteps; } }
    /// <summary>
    /// The percent of how tall each terrace will be
    /// </summary>
    public float verticalTerraceStepSize { get { return 1f / (terracesPerSlope + 1); } }

    /// <summary>
    /// Per-generated noise used for perturbing
    /// </summary>
    public static Texture2D noiseSource;

    /// <summary>
    /// How mixed up the hexes looked when perturbeds
    /// </summary>
    [SerializeField]
    [Tooltip("Amount of contorting the hex mesh will have")]
    private float _cellPerturbStrength = 2f;
    public float cellPerturbStrength { get { return _cellPerturbStrength; } }
    /// <summary>
    /// How shifted eached elevation level will be going up
    /// </summary>
    [SerializeField]
    [Tooltip("Amount of elevation will be altered")]
    private float _elevationPerturbStrength = 1.2f;
    public float elevationPerturbStrength { get { return _elevationPerturbStrength; } }

    /// <summary>
    /// How much the vertices are altered
    /// </summary>
    [SerializeField]
    [Tooltip("The effect the perturb will have")]
    private float _noiseScale = .003f;
    public float noiseScale { get { return _noiseScale; } }

    [SerializeField]
    [Tooltip("How deep the rivers will be inset")]
    private float _streamBedElevationOffset = -1f;
    public float streamBedElevationOffset { get { return _streamBedElevationOffset; } }

    [SerializeField]
    [Tooltip("The amount of the top of the river will be inset")]
    private float _waterSurfaceElevationOffset = -2f;
    public float waterSurfaceElevationOffset { get { return _waterSurfaceElevationOffset; } }

    [SerializeField]
    [Tooltip("How many chunks are in the X direction")]
    private int _chunkSizeX = 5;
    public int chunkSizeX { get { return _chunkSizeX; } }
    [SerializeField]
    [Tooltip("How many chunks are in the Z direction")]
    private int _chunkSizeY = 5;
    public int chunkSizeZ { get { return _chunkSizeY; } }

    [SerializeField]
    [Tooltip("The size of the array which will hold random values")]
    private int _hashGridSize = 256;
    private HexHash[] _hashGrid;

    [SerializeField]
    [Tooltip("The rate that the hash grid is scaled down by")]
    private float _hashGridScale = 0.25f;

    [SerializeField]
    private float[][] _featureThresholds =
    {
        new float[] {0.0f, 0.0f, 0.4f},
        new float[] {0.0f, 0.4f, 0.6f},
        new float[] {0.4f, 0.6f, 0.8f}
    };

    /// <summary>
    /// Pre-defined corners of the hexes
    /// </summary>
    private Vector3[] corners
    {
        get
        {
            return new Vector3[]
            {
                new Vector3(0f, 0f, outerRadius),
                new Vector3(innerRadius, 0f, 0.5f * outerRadius),
                new Vector3(innerRadius, 0f, -0.5f * outerRadius),
                new Vector3(0f, 0f, -outerRadius),
                new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
                new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
                new Vector3(0f, 0f, outerRadius) // Dup of first to handle OutOfRange
            };
        }
    }

    /// <summary>
    /// Gets the first corner clockwise in the direction
    /// </summary>
    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return Instance.corners[(int)direction];
    }
    /// <summary>
    /// Gets the second corner clockwise in the direction
    /// </summary>
    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return Instance.corners[(int)direction + 1];
    }
    /// <summary>
    /// Gets the first corner clockwise in the direction reduced by how far the fill percent is for the vertex colorings
    /// </summary>
    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return Instance.corners[(int)direction] * Instance.solidFactor;
    }
    /// <summary>
    /// Gets the second corner clockwise in the direction reduced by how far the fill percent is for the vertex colorings
    /// </summary>
    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return Instance.corners[(int)direction + 1] * Instance.solidFactor;
    }

    public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
    {
        return (Instance.corners[(int)direction] + Instance.corners[(int)direction + 1]) * (0.5f * Instance.solidFactor);
    }

    public static Vector3 GetFirstWaterCorner(HexDirection direction)
    {
        return Instance.corners[(int)direction] * Instance.waterFactor;
    }

    public static Vector3 GetSecondWaterCorner(HexDirection direction)
    {
        return Instance.corners[(int)direction + 1] * Instance.waterFactor;
    }

    /// <summary>
    /// Gets the position between two hexes at the extents of the fill percent using the leftover
    /// </summary>
    public static Vector3 GetBridge(HexDirection direction)
    {
        return (Instance.corners[(int)direction] + Instance.corners[(int)direction + 1]) * Instance.blendFactor;
    }

    public static Vector3 GetWaterBridge(HexDirection direction)
    {
        return (Instance.corners[(int)direction] + Instance.corners[(int)direction + 1]) * Instance.waterBlendFactor;
    }

    /// <summary>
    /// Returns the position were the terrace vertex would be
    /// </summary>
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * Instance.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * Instance.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }
    /// <summary>
    /// Returns the Color the vertex the terrace would be at
    /// </summary>
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * Instance.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    /// <summary>
    /// Figures out which edge type we should we when creating the mesh
    /// </summary>
    public static HexEdgeType GetEdgeType(float e1, float e2)
    {
        e1 = Mathf.RoundToInt(e1);
        e2 = Mathf.RoundToInt(e2);
        if (e1 == e2)
            return HexEdgeType.Flat;

        int delta = (int)e2 - (int)e1;
        if (delta == 1 || delta == -1)
            return HexEdgeType.Slope;

        return HexEdgeType.Cliff;
    }

    /// <summary>
    /// Gets the value of the noise at position
    /// </summary>
    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * Instance.noiseScale, position.z * Instance.noiseScale);
    }

    public static Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * HexMetrics.Instance.cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * HexMetrics.Instance.cellPerturbStrength;
        return position;
    }

    public static void InitializeHashGrid(int seed)
    {
        Instance._hashGrid = new HexHash[Instance._hashGridSize * Instance._hashGridSize];

        Random.State currentState = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < Instance._hashGrid.Length; i++)
        {
            Instance._hashGrid[i] = HexHash.Create();
        }

        Random.state = currentState;
    }

    public static HexHash SampleHashGrid(Vector3 position)
    {
        int x = (int)(position.x * Instance._hashGridScale) % Instance._hashGridSize;
        if (x < 0) x += Instance._hashGridSize;

        int z = (int)(position.z * Instance._hashGridScale) % Instance._hashGridSize;
        if (z < 0) z += Instance._hashGridSize;

        return Instance._hashGrid[x + z * Instance._hashGridSize];
    }

    public float[] GetFeatureThresholds(int level)
    {
        return Instance._featureThresholds[level];
    }
}
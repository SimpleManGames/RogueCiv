using UnityEngine;

/// <summary>
/// Defines the different types of linked meshes we can use
/// </summary>
public enum HexEdgeType
{
    Flat, Slope, Cliff
}

public static class HexMetrics
{
    /// <summary>
    /// Radius that defines the corners of the Hex
    /// </summary>
    public const float outerRadius = 10f;
    /// <summary>
    /// Radius that is the extents of the edges
    /// </summary>
    public const float innerRadius = outerRadius * 0.866025404f;

    /// <summary>
    /// Fill percent of the Hex before the bleed to the next cell starts
    /// </summary>
    public const float solidFactor = 0.8f;
    /// <summary>
    /// Leftover percent of the solid factor which is used for the distance that we need to bleed
    /// </summary>
    public const float blendFactor = 1f - solidFactor;

    /// <summary>
    /// This defines how large the distance between the tops of the hexes are
    /// </summary>
    public const float elevationStep = .5f * outerRadius;

    /// <summary>
    /// How many terraces will be made when the conditions to generate them happen
    /// </summary>
    public const int terracesPerSlope = 2;
    /// <summary>
    /// The actual value we use during generation
    /// </summary>
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    /// <summary>
    /// The percent of how wide each terrace will be
    /// </summary>
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    /// <summary>
    /// The percent of how tall each terrace will be
    /// </summary>
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

    /// <summary>
    /// Per-generated noise used for perturbing
    /// </summary>
    public static Texture2D noiseSource;

    /// <summary>
    /// How mixed up the hexes looked when perturbeds
    /// </summary>
    public const float cellPerturbStrength = 2f;
    /// <summary>
    /// How much the vertices are altered
    /// </summary>
    public const float noiseScale = .003f;
    /// <summary>
    /// How shifted eached elevation level will be going up
    /// </summary>
    public const float elevationPerturbStrength = 1.2f;

    public const int chunkSizeX = 5;
    public const int chunkSizeZ = 5;

    /// <summary>
    /// Pre-defined corners of the hexes
    /// </summary>
    private static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius) // Dup of first to handle OutOfRange
    };

    /// <summary>
    /// Gets the first corner clockwise in the direction
    /// </summary>
    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }
    /// <summary>
    /// Gets the second corner clockwise in the direction
    /// </summary>
    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }
    /// <summary>
    /// Gets the first corner clockwise in the direction reduced by how far the fill percent is for the vertex colorings
    /// </summary>
    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }
    /// <summary>
    /// Gets the second corner clockwise in the direction reduced by how far the fill percent is for the vertex colorings
    /// </summary>
    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    /// <summary>
    /// Gets the position between two hexes at the extents of the fill percent using the leftover
    /// </summary>
    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }

    /// <summary>
    /// Returns the position were the terrace vertex would be
    /// </summary>
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }
    /// <summary>
    /// Returns the Color the vertex the terrace would be at
    /// </summary>
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    /// <summary>
    /// Figures out which edge type we should we when creating the mesh
    /// </summary>
    public static HexEdgeType GetEdgeType(int e1, int e2)
    {
        if (e1 == e2)
            return HexEdgeType.Flat;

        int delta = e2 - e1;
        if (delta <= 1 || delta >= -1)
            return HexEdgeType.Slope;

        return HexEdgeType.Cliff;
    }

    /// <summary>
    /// Gets the value of the noise at position
    /// </summary>
    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
    }
}
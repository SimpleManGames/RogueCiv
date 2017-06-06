using System.Collections.Generic;
using UnityEngine;

public class HexGrid : Singleton<HexGrid>
{
    public HexObject hexPrefab = new HexObject();

    public HexGridChunk chunkPrefab;

    #region Properties

    public HashSet<HexObject> Hexes
    {
        get; private set;
    }
    public HexGridChunk[] chunks
    {
        get; private set;
    }

    public int ChunkCountX
    {
        get { return GlobalMapSettings.Instance.ChunkCountX; }
    }

    public int ChunkCountZ
    {
        get { return GlobalMapSettings.Instance.ChunkCountZ; }
    }

    private int width;
    public int Width
    {
        get { return GlobalMapSettings.Instance.Width; }
    }

    private int height;
    public int Height
    {
        get { return GlobalMapSettings.Instance.Height; }
    }

    private HexMesh hexMesh;
    private HexMesh HexMesh
    {
        get
        {
            if (hexMesh == null)
                hexMesh = GetComponentInChildren<HexMesh>();

            return hexMesh;
        }
    }

    public Texture2D noiseSource;

    #endregion

    private void CreateChunks()
    {
        chunks = new HexGridChunk[ChunkCountX * ChunkCountZ];

        for (int z = 0, i = 0; z < ChunkCountZ; z++)
        {
            for (int x = 0; x < ChunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells()
    {
        for (int z = 0, i = 0; z < Height; z++)
            for (int x = 0; x < Width; x++)
                CreateCell(x, z, i++);
    }

    /// <summary>
    /// Main function for creating the objects in the grid
    /// </summary>
    private void CreateCell(int x, int z, int i)
    {
        Vector3 position = new Vector3(((x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f), 0f, -(z * (HexMetrics.outerRadius * 1.5f)));
        HexObject hexObject = Hexes.Add<HexObject>(Instantiate(hexPrefab));
        hexObject.Index = i;
        hexObject.Hex = new Hex(CubeCoord.OddRowToCube(new OffsetCoord(x, z)));
        hexObject.Color = MapGenerator.Instance.colorMap[i];
        hexObject.transform.localPosition = position;
        hexObject.name += " " + hexObject.Hex.cubeCoords.ToString();
        hexObject.Elevation = MapGenerator.Instance.heightMap[x, z];

        AddHexToChunk(x, z, hexObject);
    }

    private void AddHexToChunk(int x, int z, HexObject hex)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * ChunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddHex(localX + localZ * HexMetrics.chunkSizeX, hex);
    }

    new public void Awake()
    {
        base.Awake();
        HexMetrics.noiseSource = noiseSource;

        Hexes = new HashSet<HexObject>();
        CreateChunks();
        CreateCells();
    }
}
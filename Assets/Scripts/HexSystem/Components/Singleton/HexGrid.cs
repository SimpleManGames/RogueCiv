using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGrid : Singleton<HexGrid>
{
    public HexObject hexPrefab;

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

    private Vector3 Size
    {
        get
        {
            return new Vector3((ChunkCountX * HexMetrics.Instance.chunkSizeX - 0.5f) * (2f * HexMetrics.Instance.innerRadius), 0f, -(ChunkCountZ * HexMetrics.Instance.chunkSizeZ - 1) * (1.5f * HexMetrics.Instance.outerRadius));
        }
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

    private void CreateRivers(MapSettingsData data)
    {
        System.Random prng = new System.Random(data.noiseSetting.seed);
        var terrainOnlyHexes = Hexes.ToList().Where(h => h.Elevation > 0);

        for (int i = 0; i < data.riverCount; i++)
        {
            int riverLength = prng.Next((int)data.riverLengthMinMax[0], (int)data.riverLengthMinMax[1]);

            // Start the river
            HexObject currentHex = terrainOnlyHexes.ToArray()[prng.Next(0, terrainOnlyHexes.Count())];
            // Get neighbour to connect to
            HexObject nextLowestNeighbour = Instance.FindHexObject(Hex.Neighbours(currentHex.Hex).OrderBy(n => Instance.FindHexObject(n.cubeCoords).Elevation).FirstOrDefault().cubeCoords);

            for (int l = 0; l < riverLength; l++)
            {
                if (currentHex.Elevation == 0)
                    break;

                if (currentHex == nextLowestNeighbour)
                    continue;

                currentHex.SetOutgoingRiver(Hex.Direction(currentHex.Hex, nextLowestNeighbour.Hex));
                if (nextLowestNeighbour == null)
                    break;

                currentHex = nextLowestNeighbour;
                if (currentHex == null)
                    break;

                try
                {
                    var neighbours = Hex.Neighbours(currentHex.Hex);
                    var hexObjectToFind = neighbours.OrderBy(n => Instance.FindHexObject(n.cubeCoords).Elevation).FirstOrDefault();
                    nextLowestNeighbour = Instance.FindHexObject(hexObjectToFind.cubeCoords);
                }
                catch
                {
                    continue;
                }
            }
        }
    }

    private void CreateCells()
    {
        for (int z = 0, i = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    /// <summary>
    /// Main function for creating the objects in the grid
    /// </summary>
    private void CreateCell(int x, int z, int i)
    {
        Vector3 position = new Vector3(((x + z * 0.5f - z / 2) * HexMetrics.Instance.innerRadius * 2f), 0f, -(z * (HexMetrics.Instance.outerRadius * 1.5f)));
        HexObject hexObject = Hexes.Add<HexObject>(Instantiate(hexPrefab));
        hexObject.Index = i;
        hexObject.Hex = new Hex(CubeCoord.OddRowToCube(new OffsetCoord(x, z)));
        hexObject.Color = MapGenerator.Instance.colorMap[i];
        hexObject.transform.localPosition = position;
        hexObject.name += " " + hexObject.Hex.cubeCoords.ToString();
        hexObject.Elevation = MapGenerator.Instance.heightMap[x, z];
        hexObject.WaterLevel = (hexObject.Elevation <= 0.4f) ? .1f : 0f;
        NavigationField.Instance.NavField[NavigationField.LayerType.Walkable][i] = NavigationField.Instance.walkableCurve.Evaluate(MapGenerator.Instance.heightMap[x, z] / MapGenerator.Instance.mapSettings.heightMultiplier);

        AddHexToChunk(x, z, hexObject);
    }

    private void AddHexToChunk(int x, int z, HexObject hex)
    {
        int chunkX = x / HexMetrics.Instance.chunkSizeX;
        int chunkZ = z / HexMetrics.Instance.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * ChunkCountX];

        int localX = x - chunkX * HexMetrics.Instance.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.Instance.chunkSizeZ;
        chunk.AddHex(localX + localZ * HexMetrics.Instance.chunkSizeX, hex);
    }

    new public void Awake()
    {
        base.Awake();
        HexMetrics.noiseSource = noiseSource;

        Hexes = new HashSet<HexObject>();
        CreateChunks();
        CreateCells();
        CreateRivers(MapGenerator.Instance.mapSettings);

        chunks.ToList().ForEach(x => x.Refresh());
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(Size.x / 2f, MapGenerator.Instance.mapSettings.heightMultiplier / 2f, Size.z / 2f), new Vector3(Size.x, MapGenerator.Instance.mapSettings.heightMultiplier, Size.z));
    }
}
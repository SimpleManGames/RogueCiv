using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGrid : Singleton<HexGrid>
{
    public HexObject hexPrefab = new HexObject();

    #region Properties

    public HashSet<HexObject> Hexes
    {
        get; private set;
    }

    [SerializeField]
    private int width;
    public int Width
    {
        get { return width; }
        private set { width = value; }
    }

    [SerializeField]
    private int height;
    public int Height
    {
        get { return height; }
        private set { height = value; }
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

    /// <summary>
    /// Main function for creating the objects in the grid
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="i"></param>
    private void CreateCell(int x, int z, int i)
    {
        Vector3 position = new Vector3(((x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f), 0f, -(z * (HexMetrics.outerRadius * 1.5f)));
        HexObject hexObject = Hexes.Add<HexObject>(Instantiate(hexPrefab));
        hexObject.Index = i;
        hexObject.Hex = new Hex(CubeCoord.OddRowToCube(new OffsetCoord(x, z)));
        hexObject.Color = MapGenerator.Instance.colorMap[i];
        hexObject.transform.SetParent(transform, false);
        hexObject.transform.localPosition = position;
        hexObject.name += " " + hexObject.Hex.cubeCoords.ToString();
        hexObject.Elevation = MapGenerator.Instance.heightMap[x, z];
    }

    new public void Awake()
    {
        base.Awake();

        HexMetrics.noiseSource = noiseSource;

        Hexes = new HashSet<HexObject>();

        for (int z = 0, i = 0; z < height; z++)
            for (int x = 0; x < width; x++)
                CreateCell(x, z, i++);
    }

    public void Start()
    {
        HexMesh.Triangulate(Hexes.ToArray());
    }
}
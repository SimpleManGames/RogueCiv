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

    #region Statics

    /// <summary>
    /// Gets a HexObject based off it's index
    /// </summary>
    /// <param name="index">Index of the wanted HexObject</para>
    /// <returns>HexObject with the requested Index</returns>
    public static HexObject FindHexObject(int index)
    {
        return Instance.Hexes.Where(h => h.Index == index).FirstOrDefault();
    }

    /// <summary>
    /// Finds an HexObject component with the same cube coordinates
    /// </summary>
    /// <param name="c">The location of the tile you want in cubecoords</param>
    /// <returns>HexObject Component</returns>
    public static HexObject FindHexObject(CubeCoord c)
    {
        return FindHexObject(c.Q, c.R, c.S);
    }

    /// <summary>
    /// Finds an HexObject component with the same cube coorinates
    /// </summary>
    /// <param name="q">Represents the X position</param>
    /// <param name="r">Represents the Y position</param>
    /// <param name="s">Represents the Z position</param>
    /// <returns>HexObject Component</returns>
    public static HexObject FindHexObject(double q, double r, double s)
    {
        return Instance.Hexes.Where(t => t.Hex.cubeCoords.Q == q && t.Hex.cubeCoords.R == r && t.Hex.cubeCoords.S == s).FirstOrDefault();
    }

    /// <summary>
    /// Gets all the Hexes in a range around the center
    /// </summary>
    /// <param name="center">The Hex that will be checked around</param>
    /// <param name="range">The amount of tiles away to be included</param>
    /// <returns>A List of the Hexes</returns>
    public static List<Hex> HexesInRange(Hex center, int range)
    {
        List<Hex> ret = new List<Hex>();

        CubeCoord c;

        for (int dx = -range; dx <= range; dx++)
            for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                c = new CubeCoord(dx, dy, -dx - dy) + center.cubeCoords;
                if (Instance.Hexes.Contains(FindHexObject(c)))
                    ret.Add(new Hex(c));
            }

        return ret;
    }

    /// <summary>
    /// Gets all the Hexes in a range around the coord
    /// </summary>
    /// <param name="coord">The position in Cube Coord to check around</param>
    /// <param name="range">The amount of tiles away to be included</param>
    /// <returns>A List of the Hexes</returns>
    public static List<Hex> HexesInRange(CubeCoord coord, int range)
    {
        return HexesInRange(new Hex(coord), range);
    }

    /// <summary>
    /// Gets all the Hexes in a range around the position in cube coord
    /// </summary>
    /// <param name="q">X value in a cube coord</param>
    /// <param name="r">Y value in a cube coord</param>
    /// <param name="s">Z value in a cube coord</param>
    /// <param name="range">The amount of tiles away to be included</param>
    /// <returns></returns>
    public static List<Hex> HexesInRange(double q, double r, double s, int range)
    {
        return HexesInRange(new Hex(q, r, s), range);
    }

    #endregion

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
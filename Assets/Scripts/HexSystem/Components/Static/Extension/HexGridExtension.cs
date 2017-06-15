using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HexGridExtension
{
    /// <summary>
    /// Gets a HexObject based off it's index
    /// </summary>
    /// <param name="index">Index of the wanted HexObject</para>
    /// <returns>HexObject with the requested Index</returns>
    public static HexObject FindHexObject(this HexGrid Instance, int index)
    {
        return Instance.Hexes.Where(h => h.Index == index).FirstOrDefault();
    }

    /// <summary>
    /// Finds an HexObject component with the same cube coordinates
    /// </summary>
    /// <param name="c">The location of the tile you want in cubecoords</param>
    /// <returns>HexObject Component</returns>
    public static HexObject FindHexObject(this HexGrid Instance, CubeCoord c)
    {
        return Instance.Hexes.ToList().Find(h => h.Hex.cubeCoords == c);
    }

    /// <summary>
    /// Finds an HexObject component with the same cube coorinates
    /// </summary>
    /// <param name="q">Represents the X position</param>
    /// <param name="r">Represents the Y position</param>
    /// <param name="s">Represents the Z position</param>
    /// <returns>HexObject Component</returns>
    public static HexObject FindHexObject(this HexGrid Instance, double q, double r, double s)
    {
        return Instance.FindHexObject(new CubeCoord(q, r, s));
    }

    public static HexObject[] FindHexObjects(this HexGrid Instance, Hex[] hexes)
    {
        HexObject[] retVal = new HexObject[hexes.Length];
        for (int i = 0; i < retVal.Length; i++)
            retVal[i] = Instance.FindHexObject(hexes[i].cubeCoords);

        return retVal;
    }

    /// <summary>
    /// Gets all the Hexes in a range around the center
    /// </summary>
    /// <param name="center">The Hex that will be checked around</param>
    /// <param name="range">The amount of tiles away to be included</param>
    /// <returns>A List of the Hexes</returns>
    public static List<Hex> HexesInRange(this HexGrid Instance, Hex center, int range)
    {
        List<Hex> ret = new List<Hex>();

        CubeCoord c;

        for (int dx = -range; dx <= range; dx++)
            for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                c = new CubeCoord(dx, dy, -dx - dy) + center.cubeCoords;
                if (Instance.Hexes.Contains(FindHexObject(Instance, c)))
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
    public static List<Hex> HexesInRange(this HexGrid Instance, CubeCoord coord, int range)
    {
        return HexesInRange(Instance, new Hex(coord), range);
    }

    /// <summary>
    /// Gets all the Hexes in a range around the position in cube coord
    /// </summary>
    /// <param name="q">X value in a cube coord</param>
    /// <param name="r">Y value in a cube coord</param>
    /// <param name="s">Z value in a cube coord</param>
    /// <param name="range">The amount of tiles away to be included</param>
    /// <returns></returns>
    public static List<Hex> HexesInRange(this HexGrid Instance, double q, double r, double s, int range)
    {
        return HexesInRange(Instance, new Hex(q, r, s), range);
    }
}
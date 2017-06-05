using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Hex
{
    public CubeCoord cubeCoords;

    #region Constructors

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="q">X value in a Cube Coordinate system</param>
    /// <param name="r">Y value in a Cube Coordinate system</param>
    /// <param name="s">Z value in a Cube Coordinate system</param>
    public Hex(double q, double r, double s)
    {
        Debug.Assert(q + r + s == 0);
        cubeCoords = new CubeCoord(q, r, s);
    }

    /// <summary>
    /// Offset Coord Convert Constructor
    /// </summary>
    /// <param name="x">Column</param>
    /// <param name="y">Row</param>
    public Hex(double x, double y) : this(x, y, -x - y)
    {
        // Empty
    }

    /// <summary>
    /// Predefined CubeCoord Constructor
    /// </summary>
    /// <param name="c">The CubeCoord to take values from</param>
    public Hex(CubeCoord c) : this(c.Q, c.R, c.S)
    {
        // Empty
    }

    /// <summary>
    /// Copy Constructor
    /// </summary>
    /// <param name="hex">The Hex to copy from</param>
    public Hex(Hex hex) : this(hex.cubeCoords.Q, hex.cubeCoords.R, hex.cubeCoords.S)
    {
        // Empty
    }

    #endregion

    #region Statics

    /// <summary>
    /// Holds information about the different directions in Cube Coordinate system
    /// </summary>
    public static List<Hex> Directions = new List<Hex>()
    {
        new Hex(1, 0, -1), // NE
        new Hex(1, -1, 0), // E
        new Hex(0, -1, 1), // SE
        new Hex(-1, 0 , 1), // SW
        new Hex(-1, 1, 0), // W
        new Hex(0, 1, -1) // NW
    };

    /// <summary>
    /// Gets the direction in cube coordinate system using location in the Directions List
    /// </summary>
    /// <param name="direction">Value of 0-5. 0 = Top Right going clock wise </param>
    /// <returns>The Direction requested</returns>
    public static Hex Direction(byte direction)
    {
        Debug.Assert(0 <= direction && direction < 6);
        return Directions[direction];
    }

    /// <summary>
    /// Gets the direction in cube coord system using the HexDirection enum
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Hex Direction(HexDirection direction)
    {
        return Direction((byte)direction);
    }

    /// <summary>
    /// Gets a neighbour based on a direction
    /// </summary>
    /// <param name="hex">Current Hex</param>
    /// <param name="direction">Direction of the wanted neighbour</param>
    /// <returns>The neighbour the was requested</returns>
    public static Hex Neighbour(Hex hex, byte direction)
    {
        return hex + Direction(direction);
    }

    /// <summary>
    /// Gets all the neighbours of a Hex
    /// </summary>
    /// <param name="hex">Current Hex</param>
    /// <returns>All other hexes surrounding the hex</returns>
    public static Hex[] Neighbours(Hex hex)
    {
        return Directions.Select(d => hex + d).ToArray();
    }

    /// <summary>
    /// Gets the length of a hex from origin
    /// </summary>
    /// <param name="hex">Hex to check</param>
    /// <returns>The length</returns>
    public static int Length(Hex hex)
    {
        return (int)(Mathf.Abs((float)hex.cubeCoords.Q) + Mathf.Abs((float)hex.cubeCoords.R) + Mathf.Abs((float)hex.cubeCoords.S)) / 2;
    }

    /// <summary>
    /// Gets the distance between to hexes
    /// </summary>
    /// <param name="a">Start</param>
    /// <param name="b">End</param>
    /// <returns>The length between them</returns>
    public static int Distance(Hex a, Hex b)
    {
        return Length(a - b);
    }

    #endregion

    #region Operators

    public static Hex operator +(Hex a, Hex b)
    {
        return new Hex(a.cubeCoords + b.cubeCoords);
    }

    public static Hex operator -(Hex a, Hex b)
    {
        return new Hex(a.cubeCoords - b.cubeCoords);
    }

    public static bool operator ==(Hex a, Hex b)
    {
        return a.cubeCoords == b.cubeCoords;
    }

    public static bool operator !=(Hex a, Hex b)
    {
        return a.cubeCoords != b.cubeCoords;
    }

    #endregion

    #region Overrides

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Hex o = (Hex)obj;
        if ((System.Object)o == null) return false;
        return cubeCoords.Equals(obj);
    }

    public override int GetHashCode()
    {
        return cubeCoords.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("[<color=green>{0}</color>, <color=purple>{1}</color>, <color=blue>{2}</color>]", cubeCoords.Q, cubeCoords.R, cubeCoords.S);
    }

    #endregion
}
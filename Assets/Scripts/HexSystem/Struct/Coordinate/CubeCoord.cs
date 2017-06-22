using System;
using UnityEngine;

/// <summary>
/// Contains a Q-R-S position to represent a Cube Coordinate style hex map
/// </summary>
[Serializable]
public struct CubeCoord
{
    #region Properties

    [SerializeField]
    private double q;
    public double Q
    {
        get { return q; }
        private set { q = value; }
    }
    public double X
    {
        get { return q; }
        private set { q = value; }
    }

    [SerializeField]
    private double r;
    public double R
    {
        get { return r; }
        private set { r = value; }
    }
    public double Y
    {
        get { return r; }
        private set { r = value; }
    }

    [SerializeField]
    private double s;
    public double S
    {
        get { return s; }
        private set { s = value; }
    }
    public double Z
    {
        get { return s; }
        private set { s = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="q">X value in a Cube Coordinate system</param>
    /// <param name="r">Y value in a Cube Coordinate system</param>
    /// <param name="s">Z value in a Cube Coordinate system</param>
    public CubeCoord(double q, double r, double s)
    {
        Debug.Assert(q + r + s == 0, "The values must add up to zero for the cube coordinate system to work/nValues are Q: " + q + " R: " + r + " S: " + s);
        this.q = q;
        this.r = r;
        this.s = s;
    }

    /// <summary>
    /// Offset Coord Convert Constructor
    /// </summary>
    /// <param name="x">X position from a Offset coord</param>
    /// <param name="z">Y position from a Offset coord</param>
    public CubeCoord(double x, double z) : this(x, z, -x - z)
    {
        // Empty
    }

    /// <summary>
    /// Offset Coord Convert Constructor
    /// </summary>
    /// <param name="offset">The Offset Coord to be converted</param>
    public CubeCoord(OffsetCoord offset) : this(offset.Column, offset.Row)
    {
        // Empty
    }

    /// <summary>
    /// Copy Constructer
    /// </summary>
    /// <param name="c">The CubeCoord to copy</param>
    public CubeCoord(CubeCoord c) : this(c.q, c.r, c.s)
    {
        // Empty
    }

    #endregion

    #region Conversions

    public static OffsetCoord CubeToEvenColumn(CubeCoord c)
    {
        return new OffsetCoord((int)c.X, (int)(c.Z + (c.X + ((int)c.X & 1)) / 2));
    }

    public static CubeCoord EvenColumnToCube(OffsetCoord o)
    {
        double x, y, z;
        x = o.Column;
        z = o.Row - (o.Column + (o.Column & 1)) / 2;
        y = -x - z;
        return new CubeCoord(x, y, z);
    }

    public static OffsetCoord CubeToOddColumn(CubeCoord c)
    {
        return new OffsetCoord((int)c.X, (int)(c.Z + (c.X - ((int)c.X & 1)) / 2));
    }

    public static CubeCoord OddColumnToCube(OffsetCoord o)
    {
        double x, y, z;
        x = o.Column;
        z = o.Row - (o.Column - (o.Column & 1)) / 2;
        y = -x - z;
        return new CubeCoord(x, y, z);
    }

    public static OffsetCoord CubeToEvenRow(CubeCoord c)
    {
        return new OffsetCoord((int)(c.X + (c.Z + ((int)c.Z & 1))) / 2, (int)c.Z);
    }

    public static CubeCoord EvenRowToCube(OffsetCoord o)
    {
        double x, y, z;
        x = o.Column - (o.Row + (o.Row & 1)) / 2;
        z = o.Row;
        y = -x - z;
        return new CubeCoord(x, y, z);
    }

    public static OffsetCoord CubeToOddRow(CubeCoord c)
    {
        return new OffsetCoord((int)(c.X + (c.Z - ((int)c.Z & 1)) / 2), (int)c.Z);
    }

    /// <summary>
    /// Converts an Odd-Row style Offset coord to a Cube Coordinate
    /// </summary>
    /// <param name="x">Column</param>
    /// <param name="y">Row</param>
    /// <returns>The Offset Coord as a Cube Coord</returns>
    public static CubeCoord OddRowToCube(int x, int y)
    {
        return OddRowToCube(new OffsetCoord(x, y));
    }

    /// <summary>
    /// Converts an Odd-Row style Offset coord to a Cube Coordinate
    /// </summary>
    /// <param name="o">The Offset Coord container</param>
    /// <returns>The Offset Coord as a Cube Coord</returns>
    public static CubeCoord OddRowToCube(OffsetCoord o)
    {
        double x, y, z;

        x = o.Column - (o.Row - (o.Row & 1)) / 2;
        z = o.Row;
        y = -x - z;

        return new CubeCoord(x, y, z);
    }

    #endregion

    public static CubeCoord CubeRound(CubeCoord cube)
    {
        var rx = Mathf.Round((float)cube.Q);
        var ry = Mathf.Round((float)cube.R);
        var rz = Mathf.Round((float)cube.S);

        var x_diff = Mathf.Abs(rx - (float)cube.Q);
        var y_diff = Mathf.Abs(ry - (float)cube.R);
        var z_diff = Mathf.Abs(rz - (float)cube.S);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return new CubeCoord(rx, ry, rz);
    }

    #region Operator

    public static CubeCoord operator +(CubeCoord a, CubeCoord b)
    {
        return new CubeCoord(a.Q + b.Q, a.R + b.R, a.S + b.S);
    }

    public static CubeCoord operator -(CubeCoord a, CubeCoord b)
    {
        return new CubeCoord(a.Q - b.Q, a.R - b.R, a.S - b.S);
    }

    public static bool operator ==(CubeCoord a, CubeCoord b)
    {
        return a.Q == b.Q && a.R == b.R && a.S == b.S;
    }

    public static bool operator !=(CubeCoord a, CubeCoord b)
    {
        return a.Q != b.Q && a.R != b.R && a.S != b.S;
    }

    #endregion

    #region Overrides

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        CubeCoord o = (CubeCoord)obj;
        if ((System.Object)o == null) return false;
        return ((Q == o.Q) && (R == o.R) && (S == o.S));
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", Q, R, S);
    }

    public override int GetHashCode()
    {
        return (Q.GetHashCode() ^ (R.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5)) / 2 + (Q.GetHashCode() << 6) + (Q.GetHashCode() >> 2))));
    }

    #endregion
}

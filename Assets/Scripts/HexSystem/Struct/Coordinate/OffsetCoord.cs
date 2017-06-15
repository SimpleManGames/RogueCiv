using System;
using UnityEngine;

/// <summary>
/// Contains a X-Y position to represent an Offset style hex map
/// </summary>
[Serializable]
public struct OffsetCoord
{
    #region Properties

    private int row;
    public int Row
    {
        get { return row; }
        private set { row = value; }
    }

    private int column;
    public int Column
    {
        get { return column; }
        private set { column = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="column">X position</param>
    /// <param name="row">Y position</param>
    public OffsetCoord(int column, int row)
    {
        this.column = column;
        this.row = row;
    }

    /// <summary>
    /// Copy Constructor
    /// </summary>
    /// <param name="copy">OffsetCoord that is to be copied</param>
    public OffsetCoord(OffsetCoord copy)
    {
        column = copy.Column;
        row = copy.Row;
    }

    #endregion

    #region Operators

    public static bool operator ==(OffsetCoord a, OffsetCoord b)
    {
        return a.Column == b.Column && a.Row == b.Row;
    }

    public static bool operator !=(OffsetCoord a, OffsetCoord b)
    {
        return a.column != b.Column && a.Row != b.Row;
    }

    #endregion

    #region Overrides

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("Row: " + Row + " Column: " + Column);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        OffsetCoord o = (OffsetCoord)obj;
        if ((System.Object)o == null) return false;
        return (Row == o.Row) && (Column == o.Column);
    }

    #endregion

    public static implicit operator Vector2(OffsetCoord o)
    {
        return new Vector2(o.Row, o.Column);
    }
}
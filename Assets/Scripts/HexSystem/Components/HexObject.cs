using System;
using UnityEngine;

[Serializable]
public class HexObject : MonoBehaviour
{
    private Hex hex;
    public Hex Hex
    {
        get { return hex; }
        set { hex = value; }
    }

    private int index;
    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    private Color color;
    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
        }
    }

    private float elevation;
    public float Elevation
    {
        get { return elevation; }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2 - 1) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType((int)elevation, (int)HexGrid.FindHexObject(Hex.Neighbour(hex, (byte)direction).cubeCoords).Elevation);
    }

    public HexEdgeType GetEdgeType(HexObject other)
    {
        return HexMetrics.GetEdgeType((int)Elevation, (int)other.Elevation);
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

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
            if (color == value)
                return;

            color = value;
            Refresh();
        }
    }

    private float elevation = float.MinValue;
    public float Elevation
    {
        get { return elevation; }
        set
        {
            if (elevation == value)
                return;

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.Instance.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2 - 1) * HexMetrics.Instance.elevationPerturbStrength;
            transform.localPosition = position;
            GetComponentInChildren<Text>().text = (Mathf.Round(elevation * 100f) / 100f).ToString();
            Refresh();
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    private Canvas canvas;
    public Canvas Canvas
    {
        get
        {
            return canvas == null ? canvas = GetComponentInChildren<Canvas>() : canvas;
        }
    }

    private Text text;
    public Text Text
    {
        get
        {
            return text == null ? text = Canvas.GetComponentInChildren<Text>() : text;
        }
    }

    public HexGridChunk chunk;

    private void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            HexObject[] neighbors = HexGrid.Instance.FindHexObjects(Hex.Neighbours(Hex));
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexObject neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    } 

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, HexGrid.Instance.FindHexObject(Hex.Neighbour(hex, (byte)direction).cubeCoords).Elevation);
    }

    public HexEdgeType GetEdgeType(HexObject other)
    {
        return HexMetrics.GetEdgeType(Elevation, other.Elevation);
    }
}
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

    private HexGridChunk chunk;
    public HexGridChunk Chunk
    {
        get { return chunk; }
        set { chunk = value; }
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

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
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

            if (HasOutgoingRiver && elevation < HexGrid.Instance.FindHexObject(Hex.Neighbour(this.Hex, (byte)OutgoingRiver).cubeCoords).elevation)
                RemoveOutgoingRiver();
            if (HasIncomingRiver && elevation > HexGrid.Instance.FindHexObject(Hex.Neighbour(this.Hex, (byte)IncomingRiver).cubeCoords).elevation)
                RemoveIncomingRiver();

            Refresh();
        }
    }

    private bool hasIncomingRiver;
    public bool HasIncomingRiver
    {
        get { return hasIncomingRiver; }
    }

    private bool hasOutgoingRiver;
    public bool HasOutgoingRiver
    {
        get { return hasOutgoingRiver; }
    }

    private HexDirection incomingRiver;
    public HexDirection IncomingRiver
    {
        get { return incomingRiver; }
    }

    private HexDirection outgoingRiver;
    public HexDirection OutgoingRiver
    {
        get { return outgoingRiver; }
    }

    public bool HasRiver
    {
        get { return HasIncomingRiver || HasOutgoingRiver; }
    }

    public bool HasRiverBeginOrEnd
    {
        get { return HasIncomingRiver != HasOutgoingRiver; }
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

    private void Refresh()
    {
        if (Chunk)
        {
            Chunk.Refresh();
            HexObject[] neighbors = HexGrid.Instance.FindHexObjects(Hex.Neighbours(Hex));
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexObject neighbor = neighbors[i];
                if (neighbor != null && neighbor.Chunk != Chunk)
                {
                    neighbor.Chunk.Refresh();
                }
            }
        }
    }

    private void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return HasIncomingRiver && IncomingRiver == direction || HasOutgoingRiver && OutgoingRiver == direction;
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (HasOutgoingRiver && OutgoingRiver == direction) return;

        HexObject neighbour = HexGrid.Instance.FindHexObject(Hex.Neighbour(this.Hex, (byte)direction).cubeCoords);
        if (!neighbour || Elevation < neighbour.Elevation)
            return;

        RemoveOutgoingRiver();
        if (HasIncomingRiver && IncomingRiver == direction)
            RemoveIncomingRiver();

        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly();

        neighbour.RemoveIncomingRiver();
        neighbour.hasIncomingRiver = true;
        neighbour.incomingRiver = direction.Opposite();
        neighbour.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void RemoveIncomingRiver()
    {
        if (!HasIncomingRiver)
            return;

        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexObject neighbour = HexGrid.Instance.FindHexObject(Hex.Neighbour(this.Hex, (byte)IncomingRiver).cubeCoords);
        neighbour.hasOutgoingRiver = false;
        neighbour.RefreshSelfOnly();
    }

    public void RemoveOutgoingRiver()
    {
        if (!HasOutgoingRiver)
            return;

        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexObject neighbour = HexGrid.Instance.FindHexObject(Hex.Neighbour(this.Hex, (byte)OutgoingRiver).cubeCoords);
        neighbour.hasIncomingRiver = false;
        neighbour.RefreshSelfOnly();
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
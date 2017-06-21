﻿using System;
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

    private float elevation;
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

            ValidateRivers();

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

    public float StreamBedY
    {
        get
        {
            return (Elevation + HexMetrics.Instance.streamBedElevationOffset) * HexMetrics.Instance.elevationStep;
        }
    }

    public float RiverSurfaceY
    {
        get
        {
            return (Elevation + HexMetrics.Instance.waterSurfaceElevationOffset) * HexMetrics.Instance.elevationStep;
        }
    }

    public float WaterSurfaceY
    {
        get
        {
            return (waterLevel + HexMetrics.Instance.waterSurfaceElevationOffset) * HexMetrics.Instance.elevationStep;
        }
    }

    private float waterLevel;
    public float WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
                return;

            waterLevel = value;
            ValidateRivers();
            Refresh();
        }
    }

    public bool IsUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }

    private TextMesh text;
    public TextMesh Text
    {
        get
        {
            return text == null ? text = transform.GetComponentInChildren<TextMesh>() : text;
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
        if (!IsValidRiverDestination(neighbour))
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

    private bool IsValidRiverDestination(HexObject neighbour)
    {
        return neighbour && (Elevation >= neighbour.Elevation || waterLevel == neighbour.Elevation);
    }

    private void ValidateRivers()
    {
        if (hasOutgoingRiver && !IsValidRiverDestination(HexGrid.Instance.FindHexObject(Hex.Neighbour(this.Hex, (byte)outgoingRiver).cubeCoords)))
            RemoveOutgoingRiver();
        if (hasIncomingRiver && !HexGrid.Instance.FindHexObject(Hex.Neighbour(this.Hex, (byte)outgoingRiver).cubeCoords).IsValidRiverDestination(this))
            RemoveIncomingRiver();
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
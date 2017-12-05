using System;
using UnityEngine;

[Serializable]
public class HexObject : MonoBehaviour
{
    private Hex _hex;
    public Hex Hex
    {
        get { return _hex; }
        set { _hex = value; }
    }

    private HexGridChunk _chunk;
    public HexGridChunk Chunk
    {
        get { return _chunk; }
        set { _chunk = value; }
    }

    [SerializeField]
    private int _index;
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

    private Color _color;
    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            if (_color == value)
                return;

            _color = value;
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

    [SerializeField]
    private HexObject[] _neighbours;

    private float _elevation;
    public float Elevation
    {
        get { return _elevation; }
        set
        {
            if (_elevation == value)
                return;

            _elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.Instance.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2 - 1) * HexMetrics.Instance.elevationPerturbStrength;
            transform.localPosition = position;

            ValidateRivers();

            for (int i = 0; i < roads.Length; i++)
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                    SetRoad(i, false);

            Refresh();
        }
    }

    private bool _hasIncomingRiver;
    public bool HasIncomingRiver
    {
        get { return _hasIncomingRiver; }
    }

    private bool _hasOutgoingRiver;
    public bool HasOutgoingRiver
    {
        get { return _hasOutgoingRiver; }
    }

    private HexDirection _incomingRiver;
    public HexDirection IncomingRiver
    {
        get { return _incomingRiver; }
    }

    private HexDirection _outgoingRiver;
    public HexDirection OutgoingRiver
    {
        get { return _outgoingRiver; }
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
            return waterLevel > _elevation;
        }
    }

    [SerializeField]
    private bool[] roads;

    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
                if (roads[i]) return true;

            return false;
        }
    }

    public HexDirection RiverBeginOrEndDirection
    {
        get { return HasIncomingRiver ? IncomingRiver : OutgoingRiver; }
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
        _chunk.Refresh();
    }

    #region Neighbour Functions

    public HexObject GetNeighbour(HexDirection direction)
    {
        return _neighbours[(int)direction];
    }

    public void SetNeighbour(HexDirection direction, HexObject hex)
    {
        _neighbours[(int)direction] = hex;
        hex._neighbours[(int)direction.Opposite()] = this;
    }

    #endregion

    #region River Function

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return HasIncomingRiver && IncomingRiver == direction || HasOutgoingRiver && OutgoingRiver == direction;
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (HasOutgoingRiver && OutgoingRiver == direction) return;

        HexObject neighbour = GetNeighbour(direction);
        if (!IsValidRiverDestination(neighbour))
            return;

        RemoveOutgoingRiver();
        if (HasIncomingRiver && IncomingRiver == direction)
            RemoveIncomingRiver();

        _hasOutgoingRiver = true;
        _outgoingRiver = direction;

        neighbour.RemoveIncomingRiver();
        neighbour._hasIncomingRiver = true;
        neighbour._incomingRiver = direction.Opposite();

        SetRoad((int)direction, false);
    }

    private bool IsValidRiverDestination(HexObject neighbour)
    {
        return neighbour && (Elevation >= neighbour.Elevation || waterLevel == neighbour.Elevation);
    }

    private void ValidateRivers()
    {
        if (_hasOutgoingRiver && !IsValidRiverDestination(GetNeighbour(_outgoingRiver)))
            RemoveOutgoingRiver();
        if (_hasIncomingRiver && !GetNeighbour(_outgoingRiver).IsValidRiverDestination(this))
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

        _hasIncomingRiver = false;
        RefreshSelfOnly();

        HexObject neighbour = GetNeighbour(IncomingRiver);
        neighbour._hasOutgoingRiver = false;
        neighbour.RefreshSelfOnly();
    }

    public void RemoveOutgoingRiver()
    {
        if (!HasOutgoingRiver)
            return;

        _hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexObject neighbour = GetNeighbour(OutgoingRiver);
        neighbour._hasIncomingRiver = false;
        neighbour.RefreshSelfOnly();
    }
    
    #endregion

    #region Road Functions

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public void AddRoad(HexDirection direction)
    {
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) && GetElevationDifference(direction) <= 1)
            SetRoad((int)direction, true);
    }

    public void RemoveRoads()
    {
        var neighbours = HexGrid.Instance.FindHexObjects(Hex.Neighbours(_hex));
        for (int i = 0; i < neighbours.Length; i++)
            if (roads[i])
                SetRoad(i, false);
    }

    void SetRoad(int index, bool state)
    {
        var neighbours = HexGrid.Instance.FindHexObjects(Hex.Neighbours(_hex));
        roads[index] = state;
        neighbours[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbours[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    #endregion

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(_elevation, GetNeighbour(direction).Elevation);
    }

    public HexEdgeType GetEdgeType(HexObject other)
    {
        return HexMetrics.GetEdgeType(Elevation, other.Elevation);
    }

    public float GetElevationDifference(HexDirection direction)
    {
        float difference = _elevation - GetNeighbour(direction)._elevation;
        return difference >= 0 ? difference : -difference;
    }
}
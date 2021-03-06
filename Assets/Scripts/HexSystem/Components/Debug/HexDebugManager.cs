﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexDebugManager : Singleton<HexDebugManager>
{
    private enum DebugType
    {
        None, Index, CubeCoord, Height, River,
        Water, WaterLevel,
        Walkable, Road
    }
    private DebugType debugType = DebugType.None;

    private Text layerDebugText;

    private bool clickedButton = false;

    private void F1Menu()
    {
        if (Input.GetAxis("Debug Menu") == 1)
        {
            if (!clickedButton)
            {
                clickedButton = true;

                if (Enum.GetValues(typeof(DebugType)).Length - 1 < (int)++debugType)
                    debugType = 0;

                layerDebugText.color = Color.white;
                layerDebugText.text = debugType.ToString();

                UpdateValues();
            }
        }
        else
            clickedButton = false;
    }

    private void UpdateValues()
    {
        switch (debugType)
        {
            default:
            case DebugType.None:
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Text.gameObject.SetActive(false));
                break;
            case DebugType.Index:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.gameObject.SetActive(true);
                    h.Text.color = Color.white;
                    h.Text.text = h.Index.ToString();
                });
                break;
            case DebugType.CubeCoord:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.text = h.Hex.cubeCoords.ToString();
                });
                break;
            case DebugType.Height:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.color = Color.white;
                    h.Text.text = h.Elevation.ToString("0.##");
                });
                break;
            case DebugType.River:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.color = Color.blue;
                    h.Text.text = (h.HasIncomingRiver || h.HasOutgoingRiver) ? "River" : "";
                });
                break;
            case DebugType.Water:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.color = Color.blue;
                    h.Text.text = (h.IsUnderwater) ? "Water" : "";
                });
                break;
            case DebugType.WaterLevel:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.color = Color.blue;
                    h.Text.text = h.WaterLevel.ToString();
                });
                break;
            case DebugType.Walkable:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.color = Color.white;
                    h.Text.text = NavigationField.Instance.NavField[NavigationField.LayerType.Walkable][h.Index].ToString();
                });
                break;
            case DebugType.Road:
                HexGrid.Instance.Hexes.ToList().ForEach(h =>
                {
                    h.Text.color = Color.white;
                    h.Text.text = (h.HasRoads) ? "Road" : "";
                });
                break;
        }
    }

    new private void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        layerDebugText = GameObject.Find("Debug Text").GetComponent<Text>();
        layerDebugText.text = "";
        UpdateValues();
    }

    private void Update()
    {
        F1Menu();
    }
}
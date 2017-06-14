using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexDebugManager : Singleton<HexDebugManager>
{
    private enum DebugType
    {
        None, Index, CubeCoord, Height
    }
    private DebugType debugType;

    private Text layerDebugText;

    private bool fadeDone = false;
    private float fadeOutTime = 3f;
    private float currentFadeOutTime = 0f;

    private void F1Menu()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            debugType++;
            if (Enum.GetValues(typeof(DebugType)).Length - 1 < (int)debugType)
                debugType = 0;

            layerDebugText.color = Color.white;
            layerDebugText.text = debugType.ToString();

            UpdateValues();
        }
    }

    private void UpdateValues()
    {
        switch (debugType)
        {
            default:
            case DebugType.None:
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Canvas.enabled = false);
                break;
            case DebugType.Index:
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Canvas.enabled = true);
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Text.text = h.Index.ToString());
                break;
            case DebugType.CubeCoord:
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Canvas.enabled = true);
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Text.text = h.Hex.cubeCoords.ToString());
                break;
            case DebugType.Height:
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Canvas.enabled = true);
                HexGrid.Instance.Hexes.ToList().ForEach(h => h.Text.text = h.Elevation.ToString());
                break;
        }
    }

    new private void Awake()
    {
        base.Awake();

        layerDebugText = GameObject.Find("Debug Text").GetComponent<Text>();
        layerDebugText.text = "";
    }

    private void Update()
    {
        F1Menu();
    }
}
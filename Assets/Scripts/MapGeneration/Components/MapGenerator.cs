using UnityEngine;

public sealed class MapGenerator : Singleton<MapGenerator>
{
    public MapDisplay display;

    public enum DrawMode { Noise, Color, FallOff };
    public DrawMode drawMode;

    [HideInInspector]
    public int mapWidth
    {
        get
        {
            return GlobalMapSettings.Instance.Width;
        }
    }
    [HideInInspector]
    public int mapHeight
    {
        get
        {
            return GlobalMapSettings.Instance.Height;
        }
    }

    public bool autoUpdate;

    public MapSettingsData mapSettings;

    public TerrainType[] regions;

    [HideInInspector]
    public float[,] heightMap;

    [HideInInspector]
    public Color[] colorMap;

    private float[,] falloffMap;
    private float[,] FalloffMap
    {
        get
        {
            if (falloffMap == null)
                falloffMap = Falloff.GenerateFalloffMap(mapWidth, mapHeight);

            return falloffMap;
        }
    }

    new public void Awake()
    {
        base.Awake();
        falloffMap = Falloff.GenerateFalloffMap(mapWidth, mapHeight);
        GenerateMap();
        ApplyHeightMultiplier(mapSettings.heightMultiplier, mapSettings.heightCurve);
    }

    public void GenerateMap()
    {
        falloffMap = Falloff.GenerateFalloffMap(mapWidth, mapHeight);
        DrawMap();
        colorMap = ColorMapGenerator.GenerateColorMap(heightMap, regions, mapSettings.useFalloff);
    }

    public void ApplyHeightMultiplier(float heightMultiplier, AnimationCurve heightCurve)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                heightMap[x, y] = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
            }
        }
    }

    public void DrawMap()
    {
        heightMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, mapSettings.noiseSetting);

        if (display != null)
        {
            if (drawMode == DrawMode.Noise)
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
            else if (drawMode == DrawMode.Color)
            {
                colorMap = ColorMapGenerator.GenerateColorMap(heightMap, regions, mapSettings.useFalloff);

                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
            else if (drawMode == DrawMode.FallOff)
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffMap));
        }
    }

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
            DrawMap();
    }

    public void OnValidate()
    {
        if (mapSettings != null)
        {
            mapSettings.OnValuesUpdated -= OnValuesUpdated;
            mapSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
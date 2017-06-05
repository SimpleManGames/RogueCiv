using UnityEngine;

public static class ColorMapGenerator
{
    public static Color[] GenerateColorMap(float[,] noiseMap, TerrainType[] regions, bool useFalloff)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        float[,] falloffMap = Falloff.GenerateFalloffMap(mapWidth, mapHeight);

        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                if (useFalloff)
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);

                for (int i = 0; i < regions.Length; i++)
                    if (noiseMap[x, y] <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
            }

        return colorMap;
    }
}
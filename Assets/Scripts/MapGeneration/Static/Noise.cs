using System;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int w, int h, NoiseSettings settings)
    {
        float[,] map = new float[w, h];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octavesOffsets = new Vector2[settings.octaves];
        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x;
            float offsetY = prng.Next(-100000, 100000) + settings.offset.y;
            octavesOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = w / 2f;
        float halfHeight = h / 2f;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float amp = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int o = 0; o < settings.octaves; o++)
                {
                    float sampleX = (x - halfWidth) / settings.scale * freq + octavesOffsets[o].x;
                    float sampleY = (y - halfHeight) / settings.scale * freq + octavesOffsets[o].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amp;

                    amp *= settings.persistance;
                    freq += settings.lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                map[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);

        return map;
    }
}

[Serializable]
public class NoiseSettings
{
    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = .6f;
    public float lacunarity = 2f;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}

using UnityEngine;

public static class Falloff
{
    public static float[,] GenerateFalloffMap(int x, int y)
    {
        float[,] map = new float[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float nX = i / (float)x * 2 - 1;
                float nY = j / (float)y * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(nX), Mathf.Abs(nY));
                try
                {
                    map[i, j] = Evaluate(value);
                }
                catch { }
            }
        }

        return map;
    }

    private static float Evaluate(float x)
    {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow((b - b * x), 1));
    }
}

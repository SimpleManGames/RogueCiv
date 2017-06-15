using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Circle
{
    public Vector3 position;
    public float radius;

    public static List<Vector3> PointsAroundCircle(int points, Circle circle)
    {
        return PointsAroundCircle(points, circle.radius, circle.position);
    }

    public static List<Vector3> PointsAroundCircle(int points, float radius, Vector3 center)
    {
        List<Vector3> retVal = new List<Vector3>();
        
        if (!Application.isPlaying)
            DebugExtension.DebugPoint(center, Color.red, .1f, 1);

        float slice = 2 * Mathf.PI / points;
        for (int i = 0; i < points; i++)
        {
            float angle = slice * i;
            float x = (radius * Mathf.Cos(angle) + center.x);
            float z = (radius * Mathf.Sin(angle) + center.z);
            retVal.Add(new Vector3(x, center.y, z));
        }

        return retVal;
    }
}
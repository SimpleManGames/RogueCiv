using UnityEngine;

public static class DebugExtension
{    
    public static void DebugPoint(Vector3 position, float scale = 1.0f, float duration = 0, bool depthTest = true)
    {
        DebugPoint(position, Color.white, scale, duration, depthTest);
    }
    public static void DebugPoint(Vector3 position, Color color, float scale = 1.0f, float duration = 0, bool depthTest = true)
    {
        color = (color == default(Color)) ? Color.white : color;

        Debug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, duration, depthTest);
        Debug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
        Debug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
    }
}
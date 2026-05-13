using UnityEngine;

public static class ScreenBounds
{
    public static float MinX { get; private set; }
    public static float MaxX { get; private set; }
    public static float MinY { get; private set; }
    public static float MaxY { get; private set; }
    public static float Width  => MaxX - MinX;
    public static float Height => MaxY - MinY;

    public static void Calculate(Camera cam)
    {
        float h = cam.orthographicSize;
        float w = h * cam.aspect;
        Vector3 c = cam.transform.position;
        MinX = c.x - w;
        MaxX = c.x + w;
        MinY = c.y - h;
        MaxY = c.y + h;
    }

    public static bool IsFullyVisible(Vector3 pos, float halfH)
        => pos.y < MaxY - halfH && pos.y > MinY + halfH;

    public static bool IsBelowScreen(Vector3 pos) => pos.y < MinY;
    public static bool IsAboveScreen(Vector3 pos) => pos.y > MaxY;
}

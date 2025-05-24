using UnityEngine;

public static class Extensions
{
    public static Vector2 XZ(this Vector3 v) {
        return new Vector2(v.x, v.z);
    }
    public static Vector2 XY(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }
    public static Vector2 XY(this Vector4 v) {
        return new Vector2(v.x, v.y);
    }
}
using UnityEngine;

internal sealed class GizmosUtil
{
    public static void DrawCircle(Vector3 center, float radius)
    {
        const int lineCount = 30;
        var dir = Vector3.forward;
        
        for (var i = 0; i <= lineCount; i++)
        {
            var from = center + radius * dir;
            dir = Quaternion.AngleAxis(i * 360f / lineCount, Vector3.up) * Vector3.forward;
            var to = center + radius * dir;
            
            Gizmos.DrawLine(from, to);
        }
    }
    
    public static void DrawFOV(Transform tm, float radius, float angle)
    {
        var pos = tm.position;
        var fov = MathUtil.CalcFieldOfView(tm, radius, angle);
        
        foreach (var view in fov)
        {
            Gizmos.DrawLine(pos, pos + view);
        }
    }
}
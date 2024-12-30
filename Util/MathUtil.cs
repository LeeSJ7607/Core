using System.Collections.Generic;
using UnityEngine;

internal sealed class MathUtil
{
    public static Vector3 CalcRandomPos(Vector3 pos, float radius)
    {
        var dist = Random.insideUnitCircle * radius;
        return pos + new Vector3(dist.x, 0, dist.y);
    }

    public static IEnumerable<Vector3> CalcFieldOfView(Transform tm, float radius, float angle)
    {
        Vector3 CalcDegree(float theta)
        {
            theta -= tm.eulerAngles.y;
            var x = Mathf.Cos(theta * Mathf.Deg2Rad) * radius;
            var z = Mathf.Sin(theta * Mathf.Deg2Rad) * radius;
            return new Vector3(x, 0, z);
        }
        
        var left = CalcDegree(90 + angle);
        var right = CalcDegree(90 - angle);
        return new [] { left, right };
    }
}
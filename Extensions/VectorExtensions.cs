using UnityEngine;

public static class VectorExtensions
{
    public static float SqrDistance(this Vector3 source, Vector3 target)
    {
        return (source - target).sqrMagnitude;
    }
}
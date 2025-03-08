using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AnchorNode : MonoBehaviour
{
    public Vector3 this[EAnchorNode type] => _anchorNodes[type].position;
    private readonly Dictionary<EAnchorNode, Transform> _anchorNodes = new();
    
    private void Awake()
    {
        foreach (var obj in transform)
        {
            var tm = (Transform)obj;

            if (Enum.TryParse<EAnchorNode>(tm.name, out var type))
            {
                _anchorNodes[type] = tm;
            }
            else
            {
                Debug.LogError($"AnchorNode Name: {tm.name} is InvalidCastException.");
            }
        }
    }
}
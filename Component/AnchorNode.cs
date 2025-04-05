using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AnchorNode : MonoBehaviour
{
    public Transform this[EAnchorNode type] => _anchorNodes[type];
    private readonly Dictionary<EAnchorNode, Transform> _anchorNodes = new();
    
    private void Awake()
    {
        var root = transform.Find(nameof(AnchorNode));
        
        foreach (var obj in root)
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
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIPathSO", menuName = "ScriptableObject/UIPathSO", order = 1)]
public sealed class UIPathSO : ScriptableObject
{
    [Serializable] public sealed class Map : SerializableDictionary<string, string> { }
    [SerializeField] private Map _map = new();
    
    public void Clear()
    {
        _map.Clear();
    }
    
    public void Add(string typeName, string fileName)
    {
        _map.TryAdd(typeName, fileName);
    }

    public string GetFileName(string typeName)
    {
        if (_map.TryGetValue(typeName, out var fileName))
        {
            return fileName;
        }

        throw new KeyNotFoundException(typeName);
    }
}
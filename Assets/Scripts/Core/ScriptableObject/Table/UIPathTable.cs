using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIPathTable", menuName = "ScriptableObject/UIPathTable", order = 1)]
public sealed class UIPathTable : ScriptableObject
{
    [Serializable] public sealed class TableDictionary : SerializableDictionary<string, string> { }
    [SerializeField] private TableDictionary _table = new();
    
    public void Clear()
    {
        _table.Clear();
    }
    
    public void Add(string typeName, string fileName)
    {
        _table.TryAdd(typeName, fileName);
    }

    public string GetFileName(string typeName)
    {
        if (_table.TryGetValue(typeName, out var fileName))
        {
            return fileName;
        }

        throw new KeyNotFoundException(typeName);
    }
}
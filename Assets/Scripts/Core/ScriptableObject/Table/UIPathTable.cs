using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIPathTable", menuName = "ScriptableObject/UIPathTable", order = 1)]
public sealed class UIPathTable : ScriptableObject
{
    [Serializable]
    public sealed class TableDictionary : SerializableDictionary<string, Table>
    {
        
    }
    
    [Serializable]
    public sealed class Table
    {
        public string Path;
        public string Address;
    }
    
    [SerializeField]
    private TableDictionary _table = new();
    
    public void Clear()
    {
        _table.Clear();
    }
    
    public void Add(string key, Table table)
    {
        _table.TryAdd(key, table);
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface IRow { }

public interface iBaseTable
{
    bool TryParse(IReadOnlyList<Dictionary<string, string>> csv_);
}

public abstract class BaseTable<T> : ScriptableObject, iBaseTable
{
    [Serializable] internal sealed class RowMap : SerializableDictionary<int, IRow> { } 
    [SerializeField] private RowMap _rowMap = new();
    
    //TODO: 리펙토링. 네이밍.
    public bool TryParse(IReadOnlyList<Dictionary<string, string>> excelMap)
    {
        if (excelMap.IsNullOrEmpty())
        {
            Debug.LogError($"{name} is Row Empty.");
            return false;
        }
        
        var serializeObject = JsonConvert.SerializeObject(excelMap);
        var rowMap = JsonConvert.DeserializeObject<List<T>>(serializeObject);
        if (rowMap.IsNullOrEmpty())
        {
            Debug.LogError($"{name} is Row Empty.");
            return false;
        }
        
        _rowMap.Clear();
        return true;
    }
}
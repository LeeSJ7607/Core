using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface IBaseTable
{
    bool IsReleaseAble { get; set; }
    bool TryParse(IReadOnlyList<Dictionary<string, string>> rows);
}

//TODO: 커스텀 에디터 필요하고, SO -> CSV 저장 기능 필요.
public abstract class BaseTable<TRow> : ScriptableObject, IBaseTable
{
    [Serializable] public sealed class Map : SerializableDictionary<int, TRow> { }
    [SerializeField] private Map _rowMap = new();
    bool IBaseTable.IsReleaseAble { get; set; }

    public bool TryParse(IReadOnlyList<Dictionary<string, string>> rows)
    {
        try
        {
            var json = JsonConvert.SerializeObject(rows);
            var jsonSerializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            var parsedRows = JsonConvert.DeserializeObject<List<TRow>>(json, jsonSerializerSettings);
            
            if (parsedRows == null)
            {
                Debug.LogError("JsonConvert.DeserializeObject returned null.");
                return false;
            }
            
            _rowMap.Clear();
            foreach (var row in parsedRows)
            {
                _rowMap.Add(GetRowKey(row), row);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{GetType().Name} Parse failed: {e}");
            return false;
        }

        return true;
    }

    public TRow GetRow(int key)
    {
        if (_rowMap.TryGetValue(key, out var row))
        {
            return row;
        }
        
        Debug.LogError($"Key: {key} not found in {GetType().Name}.");
        return default;
    }
    
    protected abstract int GetRowKey(TRow row);
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface IBaseTable
{
    bool IsReleaseAble { get; set; }
    bool TryParse(IReadOnlyList<Dictionary<string, string>> rows);
}

public abstract class BaseTable<TRow> : ScriptableObject, IBaseTable
{
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
            
            OnParse(parsedRows);
        }
        catch (Exception e)
        {
            Debug.LogError($"{GetType().Name} Parse failed: {e}");
            return false;
        }

        return true;
    }
    
    protected abstract void OnParse(List<TRow> rows);
}
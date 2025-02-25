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
        var ser = JsonConvert.SerializeObject(rows);
        
        try
        {
            OnParse(JsonConvert.DeserializeObject<List<TRow>>(ser, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore}));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }

    protected abstract void OnParse(List<TRow> rows);
}
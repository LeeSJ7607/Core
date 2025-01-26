using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface IBaseTable
{
    bool TryParse(IReadOnlyList<Dictionary<string, string>> rows);
}

public abstract class BaseTable<TRow> : ScriptableObject, IBaseTable
{
    public bool TryParse(IReadOnlyList<Dictionary<string, string>> rows)
    {
        var ser = JsonConvert.SerializeObject(rows);
        List<TRow> des = null;
        
        try
        {
            des = JsonConvert.DeserializeObject<List<TRow>>(ser);
            OnParse(des);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        return des != null;
    }

    protected abstract void OnParse(List<TRow> rows);
}
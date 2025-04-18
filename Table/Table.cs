﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface ITable
{
    bool IsReleaseAble { get; set; }
    bool TryParse(IReadOnlyList<Dictionary<string, string>> rows);
}

public abstract class Table<TRow> : ScriptableObject, ITable
{
    bool ITable.IsReleaseAble { get; set; }

    public bool TryParse(IReadOnlyList<Dictionary<string, string>> rows)
    {
        try
        {
            var ser = JsonConvert.SerializeObject(rows);
            var jsonSerializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore};
            OnParse(JsonConvert.DeserializeObject<List<TRow>>(ser, jsonSerializerSettings));
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
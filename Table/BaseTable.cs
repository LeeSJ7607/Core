using System;
using UnityEngine;

public abstract class BaseTable : ScriptableObject
{
    [Serializable] internal sealed class RowMap : SerializableDictionary<int, BaseTable> { }
    [SerializeField] private RowMap _rowMap = new();

    public bool TryParse(string[] columns, string[][] rows)
    {
        _rowMap.Clear();
        //var rowMap = JsonConvert.DeserializeObject<List<TRow>>(JsonConvert.SerializeObject(csv_));
        return true;
    }
}
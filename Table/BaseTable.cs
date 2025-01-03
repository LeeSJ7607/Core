using System;
using UnityEngine;

public abstract class BaseTable : ScriptableObject
{
    [Serializable] internal sealed class RowMap : SerializableDictionary<int, BaseTable> { }
    [SerializeField] private RowMap _rowMap = new();

    public bool TryParse()
    {
        _rowMap.Clear();
        return true;
    }
}
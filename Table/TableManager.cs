using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

internal sealed class TableManager : Singleton<TableManager> 
{
    private readonly Dictionary<Type, ITable> _tableMap = new();

    public void Release()
    {
        var releaseTableMap = _tableMap
                              .Where(_ => _.Value.IsReleaseAble)
                              .Select(_ => _.Key);

        foreach (var key in releaseTableMap)
        {
            _tableMap.Remove(key);
        }
    }

    public T Get<T>() where T : ITable
    {
        var type = typeof(T);

        if (_tableMap.TryGetValue(type, out var refTable))
        {
            return (T)refTable;
        }
        
        return Create<T>();
    }

    private T Create<T>() where T : ITable
    {
        var type = typeof(T);
        var res = (ITable)AddressableManager.Instance.Get<ScriptableObject>(type.Name);
        _tableMap.Add(type, res);

        return (T)res;
    }
}
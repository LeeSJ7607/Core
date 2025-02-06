using System;
using System.Collections.Generic;

internal sealed class TableManager : Singleton<TableManager> 
{
    private readonly Dictionary<Type, IBaseTable> _allSceneTableMap = new();
    private readonly Dictionary<Type, IBaseTable> _curSceneTableMap = new();

    public void Release()
    {
        _curSceneTableMap.Clear();
    }
}
using System;
using System.Collections.Generic;

internal sealed class TableManager : Singleton<TableManager> 
{
    private readonly Dictionary<Type, IBaseTable> _tableMap = new();

    public void Release()
    {
        //TODO: 지우지 말아야할 테이블도 존재함 (모든 씬에서 사용하는 테이블들)
        _tableMap.Clear();
    }
}
using System;
using System.Collections.Generic;

internal sealed class TableManager : Singleton<TableManager> 
{
    private readonly Dictionary<Type, IBaseTable> _tableMap = new();

    public void Release()
    {
        //TODO: 테이블 해제 로직 추가
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ItemTable : BaseTable<ItemTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
    }

    [SerializeField] private List<Row> _rows;

    protected override void OnParse(List<Row> rows)
    {
        _rows = rows;
    }
    
    public Row GetRow(int id)
    {
        return _rows.Find(row => row.Id == id);
    }
}
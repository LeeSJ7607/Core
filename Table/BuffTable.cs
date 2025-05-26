using System;

public sealed class BuffTable : BaseTable<BuffTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
        public eBuffType BuffType;
        public eBuffOverlap OverlapType;
    }

    protected override int GetRowKey(Row row) => row.Id;
}
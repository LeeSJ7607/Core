using System;

public sealed class BuffTable : BaseTable<BuffTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
        public eBuffEffect BuffEffectType;
        public eBuffOverlap BuffOverlapType;
        public eBuffStack BuffStackType;
        public int Value;
        public int MaxStackCount;
    }

    protected override int GetRowKey(Row row) => row.Id;
}
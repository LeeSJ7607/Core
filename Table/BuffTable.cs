using System;

public sealed class BuffTable : BaseTable<BuffTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
        public eBuffCategory BuffCategoryType;
        public eBuffEffect BuffEffectType;
        public eBuffOverlap BuffOverlapType;
        public eBuffStack BuffStackType;
    }

    protected override int GetRowKey(Row row) => row.Id;
}
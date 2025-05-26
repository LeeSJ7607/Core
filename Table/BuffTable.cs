using System;

public sealed class BuffTable : BaseTable<BuffTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
        public eBuffCategory BuffCategoryType;
        public eBuffOverlap BuffOverlapType;
        public eBuffEffect BuffEffectType;
    }

    protected override int GetRowKey(Row row) => row.Id;
}
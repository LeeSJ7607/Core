using System;

public sealed class EffectTable : BaseTable<EffectTable.Row>
{
    [Serializable]
    public sealed class Row
    {
        public int Id;
        public eEffectType EffectType;
        public int BuffId;
    }

    protected override int GetRowKey(Row row) => row.Id;
}
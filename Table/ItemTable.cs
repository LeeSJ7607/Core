using System;

public sealed class ItemTable : BaseTable<ItemTable.Row>
{
    [Serializable]
    public class Row : IRow
    {
        public int Id;
        public string Name;
    }
}
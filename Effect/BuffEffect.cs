internal sealed class BuffEffect : Effect
{
    public override void Apply(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        var buffTable = TableManager.GetTable<BuffTable>().GetRow(_effectTable.BuffId);
        var buff = BuffFactory.Create(buffTable.BuffEffectType);
        buff.Apply(buffTable, owner, target);
    }
}
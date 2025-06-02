internal sealed class BuffEffect : Effect
{
    protected override void SelfApply(IReadOnlyUnit owner)
    {
        var (buff, buffTable) = CreateBuffData();
        buff.Apply(buffTable, owner, owner);
    }

    protected override void ApplyToTarget(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        var (buff, buffTable) = CreateBuffData();
        buff.Apply(buffTable, owner, target);
    }

    private (Buff buff, BuffTable.Row buffTable) CreateBuffData()
    {
        var buffTable = TableManager.GetTable<BuffTable>().GetRow(_effectTable.BuffId);
        var buff = BuffFactory.Create(buffTable.BuffEffectType);
        return (buff, buffTable);
    }
}
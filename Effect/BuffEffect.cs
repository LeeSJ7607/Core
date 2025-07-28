internal sealed class BuffEffect : Effect
{
    protected override void SelfApply(IAttacker owner)
    {
        var (buff, buffTable) = CreateBuffData();
        buff.Apply((IDefender)owner, buffTable);
    }

    protected override void ApplyToTarget(IAttacker owner, IDefender target)
    {
        var (buff, buffTable) = CreateBuffData();
        buff.Apply(target, buffTable);
    }

    private (Buff buff, BuffTable.Row buffTable) CreateBuffData()
    {
        var buffTable = TableManager.GetTable<BuffTable>().GetRow(_effectTable.BuffId);
        var buff = BuffFactory.Create(buffTable.BuffEffectType);
        return (buff, buffTable);
    }
}
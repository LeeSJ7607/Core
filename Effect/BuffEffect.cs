internal sealed class BuffEffect : Effect
{
    public override void Apply(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        var buff = new Buff(_effectTable.BuffId);
        buff.Apply(owner, target);
    }
}
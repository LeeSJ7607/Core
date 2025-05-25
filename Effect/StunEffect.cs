internal sealed class StunEffect : Effect
{
    public override void Apply(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        ApplyBuff(owner, target);
    }

    private void ApplyBuff(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        var buff = new Buff(_effectTable.BuffId);
        buff.Apply(owner, target);
    }
}
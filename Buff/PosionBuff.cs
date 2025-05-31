internal sealed class PosionBuff : Buff
{
    public override void Apply(BuffTable.Row buffTable, IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        base.Apply(buffTable, owner, target);
    }
}
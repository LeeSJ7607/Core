internal abstract class Effect
{
    protected EffectTable.Row _effectTable;

    public void Initialize(EffectTable.Row effectTable)
    {
        _effectTable = effectTable;
    }

    public void Apply(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        if (target == null)
        {
            SelfApply(owner);
        }
        else
        {
            ApplyToTarget(owner, target);
        }
    }

    protected virtual void SelfApply(IReadOnlyUnit owner) { }
    protected abstract void ApplyToTarget(IReadOnlyUnit owner, IReadOnlyUnit target);
}
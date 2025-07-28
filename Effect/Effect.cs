internal abstract class Effect
{
    protected EffectTable.Row _effectTable;

    public void Initialize(EffectTable.Row effectTable)
    {
        _effectTable = effectTable;
    }

    public void Apply(IAttacker owner, IDefender target)
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

    protected virtual void SelfApply(IAttacker owner) { }
    protected abstract void ApplyToTarget(IAttacker owner, IDefender target);
}
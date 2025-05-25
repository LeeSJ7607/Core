internal abstract class Effect
{
    protected EffectTable.Row _effectTable;

    public void Initialize(EffectTable.Row effectTable)
    {
        _effectTable = effectTable;
    }
    
    public abstract void Apply(IReadOnlyUnit owner, IReadOnlyUnit target);
}
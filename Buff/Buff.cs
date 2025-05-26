//TODO: 설계중.
internal sealed class Buff
{
    private readonly int _buffId;
    
    public Buff(int buffId)
    {
        _buffId = buffId;
    }

    public void Apply(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        var buffTable = TableManager.GetTable<BuffTable>().GetRow(_buffId);

        if (buffTable.BuffOverlapType == eBuffOverlap.Ignore)
        {
            return;
        }

        switch (buffTable.BuffCategoryType)
        {
        case eBuffCategory.Buff:
            {
                owner.AddBuffEffect(buffTable.BuffEffectType);
            }
            break;

        case eBuffCategory.DeBuff:
            {
                target.AddBuffEffect(buffTable.BuffEffectType);
            }
            break;
        }
    }
}
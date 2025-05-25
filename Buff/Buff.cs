internal sealed class Buff
{
    private readonly int _buffId;
    
    public Buff(int buffId)
    {
        _buffId = buffId;
    }

    public void Apply(IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        var buffTable = DataAccessor.GetTable<BuffTable>().GetRow(_buffId);

        if (buffTable.OverlapType == eBuffOverlap.Ignore)
        {
            return;
        }

        switch (buffTable.BuffType)
        {
        case eBuffType.Buff:
            {
                owner.AddStatusEffect(eDeBuffStatus.Stun);
            }
            break;

        case eBuffType.DeBuff:
            {
                target.AddStatusEffect(eDeBuffStatus.Stun);
            }
            break;
        }
    }
}
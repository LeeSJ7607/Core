//TODO: 설계중.
public abstract class Buff
{
    public virtual void Apply(BuffTable.Row buffTable, IReadOnlyUnit owner, IReadOnlyUnit target)
    {
        if (buffTable.BuffOverlapType == eBuffOverlap.Ignore)
        {
            return;
        }

        switch (buffTable.BuffCategoryType)
        {
        case eBuffCategory.Buff:
            {
                owner.AddBuff(this);
            }
            break;

        case eBuffCategory.DeBuff:
            {
                target.AddBuff(this);
            }
            break;
        }
    }
}
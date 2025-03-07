using UnityEngine;

public sealed class SeekImpl
{
    private readonly ISeeker[] _seekers = new ISeeker[(int)ESeekRule.End]
    {
        new SeekBySelf(),
        new SeekByRandom(),
        new SeekByFarthestEnemy(),
        new SeekByNearestEnemy(),
        new SeekByHPLeastEnemy(),
        new SeekByRangeEnemy(),
        new SeekByExceptRange(),
        new SeekByNearestIfDamaged(),
        new SeekByDefenseBuilding(),
    };

    public ISeeker this[ESeekRule seekRule]
    {
        get
        {
            if (seekRule < ESeekRule.End)
            {
                return _seekers[(int)seekRule];
            }
            
            Debug.LogError($"Invalid SeekType : {seekRule}");
            return _seekers[(int)ESeekRule.NearestEnemy];
        }
    }
}
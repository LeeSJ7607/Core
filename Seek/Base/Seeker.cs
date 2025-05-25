using UnityEngine;

public sealed class Seeker
{
    //TODO: Factory (Func<ISeeker>)
    private readonly ISeeker[] _seekers = 
    {
        new SeekBySelf(),
        new SeekByRandom(),
        new SeekByFOVEnemy(),
        new SeekByFarthestEnemy(),
        new SeekByNearestEnemy(),
        new SeekByHPLeastEnemy(),
        new SeekByRangeEnemy(),
        new SeekByExceptRange(),
        new SeekByNearestIfDamaged(),
        new SeekByDefenseBuilding(),
    };

    public ISeeker this[eSeekRule seekRule]
    {
        get
        {
            if (seekRule < eSeekRule.End)
            {
                return _seekers[(int)seekRule];
            }
            
            Debug.LogError($"Invalid SeekType : {seekRule}");
            return _seekers[(int)eSeekRule.NearestEnemy];
        }
    }
}
using System.Collections.Generic;

public sealed class TargetController
{
    public IReadOnlyUnit Target { get; private set; }
    private readonly Seeker _seeker = new();
    private readonly IReadOnlyUnit _owner;
    
    public TargetController(IReadOnlyUnit owner)
    {
        _owner = owner;
    }
    
    public bool TryFindTarget(IEnumerable<IReadOnlyUnit> units, ESeekRule seekRule = ESeekRule.NearestEnemy)
    {
        var targets = _seeker[seekRule].Seek(units, _owner);
        if (targets.IsNullOrEmpty())
        {
            return false;
        }
        
        // foreach (var target in targets)
        // {
        //     var dir = target.transform.position - _owner.transform.position;
        //     if (dir.magnitude > 5f) //TODO: 테이블.
        //     {
        //         continue;
        //     }
        //
        //     var dot = Vector3.Dot(_owner.transform.forward, dir);
        //     var theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //     if (theta > 30f) //TODO: 테이블.
        //     {
        //         continue;
        //     }
        //
        //     Target = target;
        //     return true;
        // }

        Target = targets[0];
        return true;
    }
}
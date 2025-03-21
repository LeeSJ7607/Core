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
    
    public bool TryFindTarget(IEnumerable<IReadOnlyUnit> units, ESeekRule seekRule = ESeekRule.FOVEnemy)
    {
        var targets = _seeker[seekRule].Seek(units, _owner);
        if (targets.IsNullOrEmpty())
        {
            return false;
        }

        Target = targets[0];
        return true;
    }
}
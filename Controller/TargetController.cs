public sealed class TargetController
{
    public Unit Target { get; private set; }
    private readonly Seeker _seeker = new();
    private readonly Unit _owner;
    
    public TargetController(Unit owner)
    {
        _owner = owner;
    }
    
    public bool TryFindTarget(ESeekRule seekRule = ESeekRule.NearestEnemy)
    {
        var targets = _seeker[seekRule].Seek(_owner);
        if (targets == null)
        {
            return false;
        }

        Target = targets[0];
        return true;
    }
}
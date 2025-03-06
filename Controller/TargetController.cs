public sealed class TargetController
{
    public Unit Target { get; private set; }
    private readonly Unit _owner;
    
    public TargetController(Unit owner)
    {
        _owner = owner;
    }
    
    public bool TryFindTarget()
    {
        return true;
    }
}
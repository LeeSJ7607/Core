using System.Collections.Generic;

public sealed class BattleEnvironment
{
    public IEnumerable<IReadOnlyUnit> Units => _units;
    private readonly HashSet<IReadOnlyUnit> _units = new();

    public void RemoveUnit(IReadOnlyUnit unit)
    {
        _units.Remove(unit);
    }
}
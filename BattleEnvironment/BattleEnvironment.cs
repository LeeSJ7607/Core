using System.Collections.Generic;

public interface IUnitController
{
    IEnumerable<IReadOnlyUnit> Units { get; }
    IUnitInitializer CreateUnit(int unitId);
    void RemoveUnit(IReadOnlyUnit unit);
}

//TODO: 폴더 정리 (스크립트 위치)
public sealed class BattleEnvironment : IUnitController
{
    IEnumerable<IReadOnlyUnit> IUnitController.Units => _units;
    private readonly HashSet<IReadOnlyUnit> _units = new();
    private readonly UnitContainer _unitContainer = new();

    IUnitInitializer IUnitController.CreateUnit(int unitId)
    {
        var unit = _unitContainer.GetOrCreate(unitId, null);
        _units.Add((IReadOnlyUnit)unit);
        return unit;
    }
    
    void IUnitController.RemoveUnit(IReadOnlyUnit unit)
    {
        _units.Remove(unit);
    }
}
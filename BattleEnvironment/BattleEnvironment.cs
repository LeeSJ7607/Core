using System.Collections.Generic;
using UnityEngine;

public interface IUnitControllerBinder
{
    void Initialize(IUnitController unitController);
}

public interface IUnitController
{
    IEnumerable<IReadOnlyUnit> Units { get; }
    IUnitInitializer RegisterUnit(int unitId, Vector3 pos, Quaternion rot);
    void RemoveUnit(IReadOnlyUnit unit);
}

public interface IReadOnlyBattleEnvironment
{
    
}

public sealed class BattleEnvironment : 
    IReadOnlyBattleEnvironment,
    IUnitController
{
    IEnumerable<IReadOnlyUnit> IUnitController.Units => _units;
    private readonly HashSet<IReadOnlyUnit> _units = new();
    private readonly UnitContainer _unitContainer = new();

    public void Release()
    {
        _unitContainer.Release();
    }

    IUnitInitializer IUnitController.RegisterUnit(int unitId, Vector3 pos, Quaternion rot)
    {
        var unit = _unitContainer.GetOrCreate(unitId, null, pos, rot);
        _units.Add((IReadOnlyUnit)unit);
        return unit;
    }
    
    void IUnitController.RemoveUnit(IReadOnlyUnit unit)
    {
        _units.Remove(unit);
    }
}
using System.Collections.Generic;
using UnityEngine;

public interface IUnitControllerBinder
{
    void Initialize(IUnitController unitController);
}

public interface IUnitController
{
    IEnumerable<IReadOnlyUnit> Units { get; }
    IUnitInitializer RegisterUnit(int unitId, Vector3 pos, Quaternion rot, Transform root = null);
    void RemoveUnit(IReadOnlyUnit unit);
}

public sealed class UnitController : IUnitController
{
    IEnumerable<IReadOnlyUnit> IUnitController.Units => _units;
    private readonly HashSet<IReadOnlyUnit> _units = new();

    void IUnitController.RemoveUnit(IReadOnlyUnit unit)
    {
        _units.Remove(unit);
    }
    
    IUnitInitializer IUnitController.RegisterUnit(int unitId, Vector3 pos, Quaternion rot, Transform root)
    {
        var row = DataAccessor.GetTable<UnitTable>().GetRow(unitId);
        var res = AddressableManager.Instance.Get<GameObject>(row.PrefabName);
        var obj = UnityEngine.Object.Instantiate(res, root);
        obj.transform.SetPositionAndRotation(pos, rot);

        if (obj.TryGetComponent<IUnitInitializer>(out var unit))
        {
            _units.Add((IReadOnlyUnit)unit);
            return unit;
        }
        
        Debug.LogError($"Resource: {res}, Unit Component Confirm Required");
        UnityEngine.Object.Destroy(obj);
        return null;
    }
}
using System.Collections.Generic;
using UnityEngine;

public interface IUnitContainerBinder
{
    void Initialize(UnitContainer unitContainer);
}

public sealed class UnitContainer
{
    public IEnumerable<IReadOnlyUnit> Units => _units;
    private readonly HashSet<IReadOnlyUnit> _units = new();

    public void RemoveUnit(IReadOnlyUnit unit)
    {
        _units.Remove(unit);
    }
    
    public IUnitInitializer RegisterUnit(int unitId, Vector3 pos, Quaternion rot, Transform root = null)
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
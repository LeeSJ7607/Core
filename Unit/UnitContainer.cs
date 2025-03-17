using System.Collections.Generic;
using UnityEngine;

public sealed class UnitContainer
{
    private readonly Dictionary<int, IUnitInitializer> _unitMap = new();

    public void Release()
    {
        _unitMap.Clear();
    }

    public IUnitInitializer GetOrCreate(int unitId, Transform root)
    {
        if (_unitMap.TryGetValue(unitId, out var unit))
        {
            return unit;
        }

        return Create(unitId, root);
    }

    private IUnitInitializer Create(int unitId, Transform root)
    {
        var row = DataAccessor.GetTable<UnitTable>().GetRow(unitId);
        var res = AddressableManager.Instance.Get<GameObject>(row.PrefabName);

        if (UnityEngine.Object.Instantiate(res, root).TryGetComponent<IUnitInitializer>(out var unit))
        {
            _unitMap.Add(unitId, unit);
            return unit;
        }
        
        Debug.LogError($"Resource: {res}, Unit Component Confirm Required");
        return null;
    }
}
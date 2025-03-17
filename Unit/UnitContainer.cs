using System.Collections.Generic;
using UnityEngine;

public sealed class UnitContainer
{
    private readonly Dictionary<int, IUnitInitializer> _unitMap = new();

    public void Release()
    {
        _unitMap.Clear();
    }

    public IUnitInitializer GetOrCreate(int unitId, Transform root, Vector3 pos, Quaternion rot)
    {
        if (_unitMap.TryGetValue(unitId, out var unit))
        {
            return unit;
        }

        return Create(unitId, root, pos, rot);
    }

    private IUnitInitializer Create(int unitId, Transform root, Vector3 pos, Quaternion rot)
    {
        var row = DataAccessor.GetTable<UnitTable>().GetRow(unitId);
        var res = AddressableManager.Instance.Get<GameObject>(row.PrefabName);
        var obj = UnityEngine.Object.Instantiate(res, root);
        obj.transform.SetPositionAndRotation(pos, rot);

        if (obj.TryGetComponent<IUnitInitializer>(out var unit))
        {
            _unitMap.Add(unitId, unit);
            return unit;
        }
        
        Debug.LogError($"Resource: {res}, Unit Component Confirm Required");
        return null;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawner
{
    void Spawn();
}

public sealed class BotSpawner : MonoBehaviour, 
    ISpawner,
    IUnitContainerBinder
{
    [Serializable]
    private struct SpawnSetting
    {
        public float Radius;
    }
    
    [Serializable]
    private struct UnitData
    {
        public int Id;
        public int Count;
    }

    [SerializeField] private SpawnSetting _spawnSetting;
    [SerializeField] private List<UnitData> _unitDataList;
    private UnitContainer _unitContainer;

    void IUnitContainerBinder.Initialize(UnitContainer unitContainer)
    {
        _unitContainer = unitContainer;
    }
    
    void ISpawner.Spawn()
    {
        if (_unitDataList.IsNullOrEmpty())
        {
            Debug.LogError("Spawn Failed. UnitData is NullOrEmpty");
            return;
        }

        foreach (var unitData in _unitDataList)
        {
            for (var i = 0; i < unitData.Count; i++)
            {
                var pos = MathUtil.GetRandomPositionInRadius(transform.position, _spawnSetting.Radius);
                var unit = _unitContainer.RegisterUnit(unitData.Id, pos, transform.rotation);
                unit.Initialize(unitData.Id, eFaction.Enemy, _unitContainer);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        GizmosUtil.DrawCircle(transform.position, _spawnSetting.Radius);
    }
}
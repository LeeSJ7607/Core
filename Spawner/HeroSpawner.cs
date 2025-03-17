using System.Collections.Generic;
using UnityEngine;

public sealed class HeroSpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private int _unitId;
    private IUnitController _unitController;

    public void Initialize(IUnitController unitController)
    {
        _unitController = unitController;
    }
    
    void ISpawner.Spawn()
    {
        var unit = _unitController.CreateUnit(_unitId);
        unit.Initialize(_unitId, EFaction.Ally, _unitController, transform.position, transform.rotation);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        GizmosUtil.DrawCircle(transform.position, 1f);
    }
}
using UnityEngine;

public sealed class HeroSpawner : MonoBehaviour, 
    ISpawner,
    IUnitControllerBinder
{
    [SerializeField] private int _unitId;
    private IUnitController _unitController;

    void IUnitControllerBinder.Initialize(IUnitController unitController)
    {
        _unitController = unitController;
    }
    
    void ISpawner.Spawn()
    {
        var unit = _unitController.RegisterUnit(_unitId, transform.position, transform.rotation);
        unit.Initialize(_unitId, eFaction.Ally, _unitController);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        GizmosUtil.DrawCircle(transform.position, 0.1f);
    }
}
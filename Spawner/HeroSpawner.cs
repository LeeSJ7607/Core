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
        var unit = _unitController.CreateUnit(_unitId, transform.position, transform.rotation);
        unit.Initialize(_unitId, EFaction.Ally, _unitController);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        GizmosUtil.DrawCircle(transform.position, 1f);
    }
}
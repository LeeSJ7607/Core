using UnityEngine;

public sealed class HeroSpawner : MonoBehaviour, 
    ISpawner,
    IUnitContainerBinder
{
    [SerializeField] private int _unitId;
    private UnitContainer _unitContainer;

    void IUnitContainerBinder.Initialize(UnitContainer unitContainer)
    {
        _unitContainer = unitContainer;
    }
    
    void ISpawner.Spawn()
    {
        var unit = _unitContainer.RegisterUnit(_unitId, transform.position, transform.rotation);
        unit.Initialize(_unitId, eFaction.Ally, _unitContainer);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        GizmosUtil.DrawCircle(transform.position, 0.1f);
    }
}
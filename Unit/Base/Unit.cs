using UnityEngine;
using UnityEngine.AI;

public interface IReadOnlyUnit
{
    UnitTable.Row UnitTable { get; }
    EFaction FactionType { get; }
    bool IsDead { get; }
    Transform Tm { get; }
    IAnimatorController AnimatorController { get; }
}

public interface IUnitInitializer
{
    void Initialize(int unitId, EFaction factionType, IUnitController unitController);
}

[RequireComponent(typeof(NavMeshAgent), typeof(AnchorNode))]
public abstract partial class Unit : MonoBehaviour,
    IUnitInitializer,
    IReadOnlyUnit, 
    IAttacker,
    IDefender
{
#region IReadOnlyUnit
    UnitTable.Row IReadOnlyUnit.UnitTable => DataAccessor.GetTable<UnitTable>().GetRow(_unitId);
    public EFaction FactionType { get; private set; }
    public bool IsDead => _stat[EStat.HP] <= 0;
    Transform IReadOnlyUnit.Tm => transform;
    public IAnimatorController AnimatorController => _animatorController;
#endregion
    
    private int _unitId;
    private readonly Stat _stat = new();
    private readonly UnitUI _unitUI = new();
    private IUnitController _unitController;
    private DeadController _deadController;
    private AnimatorController _animatorController;
    private readonly UnitAIController _unitAIController = new();

    private void OnDisable()
    {
        _deadController.Release();
        _animatorController.Release();
        _unitAIController.Release();
    }

    protected virtual void Awake()
    {
        _animatorController = new AnimatorController(this);
        _deadController = new DeadController(this);
    }
    
    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        _unitUI.OnUpdate();
        _unitAIController.OnUpdate();
    }

    void IUnitInitializer.Initialize(int unitId, EFaction factionType, IUnitController unitController)
    {
        _unitId = unitId;
        FactionType = factionType;
        _unitController = unitController;
        _stat.Initialize(this);
        _unitUI.Initialize(this);
        _unitAIController.Initialize(this, unitController.Units);
        _deadController.Initialize();
        _animatorController.Initialize();
    }
    
    private void OnDrawGizmos()
    {
        if (_unitId == 0)
        {
            return;
        }
        
        var unitTable = (this as IReadOnlyUnit).UnitTable;
        GizmosUtil.DrawFOV(transform, unitTable.FOV_Radius, unitTable.FOV_Angle);
        Gizmos.color = Color.red;
    }
}
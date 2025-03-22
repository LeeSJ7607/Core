using R3;
using UnityEngine;
using UnityEngine.AI;

public interface IReadOnlyUnit
{
    UnitTable.Row UnitTable { get; }
    EFaction FactionType { get; }
    bool IsDead { get; }
    Transform Tm { get; }
    IAnimatorController AnimatorController { get; }
    Observable<R3.Unit> OnRelease { get; }
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
    Observable<R3.Unit> IReadOnlyUnit.OnRelease => _onRelease;
    public IAnimatorController AnimatorController => _animatorController;
#endregion
    
    private int _unitId;
    private Stat _stat;
    private UnitUI _unitUI;
    private IUnitController _unitController;
    private DeadController _deadController;
    private AnimatorController _animatorController;
    private readonly UnitAIController _unitAIController = new();
    private readonly ReactiveCommand _onRelease = new();

    private void OnDisable()
    {
        _onRelease.Execute(R3.Unit.Default);
        _onRelease.Dispose();
    }

    protected virtual void Awake()
    {
        _stat = new Stat(this);
        _unitUI = new UnitUI(this);
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
        _unitUI.Initialize();
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
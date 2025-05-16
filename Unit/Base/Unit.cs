using UnityEngine;
using UnityEngine.AI;

public interface IReadOnlyUnit
{
    eFaction FactionType { get; }
    UnitTable.Row UnitTable { get; }
    IReadOnlyStat Stat { get; }
    bool IsDead { get; }
    Transform Tm { get; }
    IAnimatorController AnimatorController { get; }
}

public interface IUnitInitializer
{
    void Initialize(int unitId, eFaction factionType, IUnitController unitController);
}

[RequireComponent(typeof(NavMeshAgent), typeof(AnchorNode))]
public abstract partial class Unit : MonoBehaviour,
    IUnitInitializer,
    IReadOnlyUnit, 
    IAttacker,
    IDefender
{
#region IReadOnlyUnit
    public eFaction FactionType { get; private set; }
    UnitTable.Row IReadOnlyUnit.UnitTable => DataAccessor.GetTable<UnitTable>().GetRow(_unitId);
    IReadOnlyStat IReadOnlyUnit.Stat => _stat;
    public bool IsDead => _stat[EStat.HP] <= 0;
    Transform IReadOnlyUnit.Tm => transform;
    public IAnimatorController AnimatorController => _animatorController;
#endregion

    public MoveController MoveController { get; private set; }
    public AttackController AttackController { get; private set; }
    public TargetController TargetController { get; private set; }
    public SkillController SkillController { get; private set; }
    
    private int _unitId;
    private readonly Stat _stat = new();
    private readonly UnitUI _unitUI = new();
    private DeadController _deadController;
    private AnimatorController _animatorController;
    protected IUnitController _unitController;

    protected virtual void OnDisable()
    {
        _deadController.Release();
        _animatorController.Release();
        AttackController.Release();
    }

    protected virtual void Awake()
    {
        _deadController = new DeadController(this);
        _animatorController = new AnimatorController(this);
        MoveController = new MoveController(this);
        TargetController = new TargetController(this);
        AttackController = new AttackController(this);
        SkillController = new SkillController();
    }
    
    void IUnitInitializer.Initialize(int unitId, eFaction factionType, IUnitController unitController)
    {
        _unitId = unitId;
        FactionType = factionType;
        _unitController = unitController;
        _stat.Initialize(this);
        _unitUI.Initialize(this);
        _deadController.Initialize();
        _animatorController.Initialize();
        SkillController.Initialize(this);
        OnInitialize();
    }
    
    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        _unitUI.OnUpdate();
    }
    
    private void OnDrawGizmos()
    {
        if (_unitId == 0)
        {
            return;
        }
        
        var unitTable = (this as IReadOnlyUnit).UnitTable;
        if (unitTable.SeekRuleType == ESeekRule.FOVEnemy)
        {
            GizmosUtil.DrawFOV(transform, unitTable.FOV_Radius, unitTable.FOV_Angle);
        }
        else
        {
            GizmosUtil.DrawCircle(transform.position, unitTable.FOV_Radius);
        }

        Gizmos.color = Color.red;
    }
    
    protected abstract void OnInitialize();
}
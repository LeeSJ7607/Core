using R3;
using UnityEngine;
using UnityEngine.AI;

public interface IAttacker
{
    int Damage { get; }
}

public interface IDefender
{
    Vector3 Pos { get; }
    void Hit(int damage);
}

public interface IReadOnlyUnit
{
    UnitTable.Row UnitTable { get; }
    EFaction FactionType { get; }
    bool IsDead { get; }
    Transform Tm { get; }
    Observable<R3.Unit> OnRelease { get; }
    IAnimatorController AnimatorController { get; }
}

public interface IUnitInitializer
{
    void Initialize(int unitId, EFaction factionType, IUnitController unitController);
}

[RequireComponent(typeof(NavMeshAgent), typeof(AnchorNode))]
public abstract class Unit : MonoBehaviour,
    IUnitInitializer,
    IReadOnlyUnit, 
    IAttacker,
    IDefender
{
    private int _unitId;
    private Stat _stat;
    private UnitUI _unitUI;
    private IUnitController _unitController;
    private readonly ReactiveCommand _onRelease = new();

#region Attacker
    int IAttacker.Damage => 100;
#endregion

#region Defender
    Vector3 IDefender.Pos => transform.position;
#endregion
    
#region IReadOnlyUnit
    UnitTable.Row IReadOnlyUnit.UnitTable => DataAccessor.GetTable<UnitTable>().GetRow(_unitId);
    public EFaction FactionType { get; private set; }
    public bool IsDead => _stat[EStat.HP] <= 0;
    Transform IReadOnlyUnit.Tm => transform;
    Observable<R3.Unit> IReadOnlyUnit.OnRelease => _onRelease;
    public IAnimatorController AnimatorController => _animatorController; //TODO: 한 군데에서만 처리하고 싶은데.. public 으로 해야하나..
#endregion

#region Controller
    private readonly UnitAIController _unitAIController = new();
    private AnimatorController _animatorController;
    private DeadController _deadController;
#endregion

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
        _animatorController.OnUpdate();
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

    void IDefender.Hit(int damage)
    {
        _stat[EStat.HP] -= damage;
        _unitUI.SetHPAndDamage(_stat, damage);
        AnimatorController.SetState(IsDead ? EAnimState.Die : EAnimState.Hit);
        
        if (IsDead)
        {
            _unitController.RemoveUnit(this);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosUtil.DrawFOV(transform, 5f, 30f);
    }
}
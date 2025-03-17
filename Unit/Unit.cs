using R3;
using UnityEngine;
using UnityEngine.AI;

public interface IAttacker
{
    int Damage { get; }
}

public interface IDefender
{
    Transform Tm { get; }
    void Hit(int damage);
}

public interface IReadOnlyUnit : IAttacker, IDefender
{
    int UnitId { get; }
    UnitTable.Row UnitTable { get; }
    EFaction FactionType { get; }
    bool IsDead { get; }
    Vector3 Pos { get; }
    Observable<R3.Unit> OnRelease { get; }
    AnimatorController AnimatorController { get; }
}

public interface IUnitInitializer
{
    void Initialize(IUnitController unitController, int unitId, EFaction factionType, Vector3 pos, Quaternion rot);
}

[RequireComponent(typeof(NavMeshAgent), typeof(AnchorNode))]
public abstract class Unit : MonoBehaviour, 
    IReadOnlyUnit, 
    IUnitInitializer
{
    private Stat _stat;
    private UnitUI _unitUI;
    private IUnitController _unitController;
    private readonly ReactiveCommand _onRelease = new();

#region Attacker
    int IAttacker.Damage => 100;
#endregion

#region Defender
    Transform IDefender.Tm => transform;
#endregion

#region IReadOnlyUnit
    public int UnitId { get; private set; }
    UnitTable.Row IReadOnlyUnit.UnitTable => DataAccessor.GetTable<UnitTable>().GetRow(UnitId);
    public EFaction FactionType { get; private set; }
    public bool IsDead => _stat[EStat.HP] <= 0;
    Vector3 IReadOnlyUnit.Pos => transform.position;
    Observable<R3.Unit> IReadOnlyUnit.OnRelease => _onRelease;
    public AnimatorController AnimatorController { get; private set; } //TODO: 한 군데에서만 처리하고 싶은데.. public 으로 해야하나..
#endregion

#region Controller
    private readonly UnitAIController _unitAIController = new();
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
        AnimatorController = new AnimatorController(this);
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
        AnimatorController.OnUpdate();
    }

    void IUnitInitializer.Initialize(IUnitController unitController, int unitId, EFaction factionType, Vector3 pos, Quaternion rot)
    {
        _unitController = unitController;
        UnitId = unitId;
        FactionType = factionType;
        transform.SetPositionAndRotation(pos, rot);
        _unitUI.Initialize();
        _unitAIController.Initialize(this, unitController.Units);
        _deadController.Initialize();
        AnimatorController.Initialize();
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
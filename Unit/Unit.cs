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
    Observable<R3.Unit> OnRelease { get; }
    Vector3 Pos { get; }
    bool IsDead { get; }
    EFaction FactionType { get; }
    AnimatorController AnimatorController { get; }
}

[RequireComponent(typeof(NavMeshAgent), typeof(AnchorNode))]
public abstract class Unit : MonoBehaviour, IReadOnlyUnit
{
    private Stat _stat;
    private UnitUI _unitUI;
    private BattleEnvironment _battleEnvironment;
    private readonly ReactiveCommand _onRelease = new();

#region Attacker
    int IAttacker.Damage => 100;
#endregion

#region Defender
    Transform IDefender.Tm => transform;
#endregion

#region IReadOnlyUnit
    Observable<R3.Unit> IReadOnlyUnit.OnRelease => _onRelease;
    Vector3 IReadOnlyUnit.Pos => transform.position;
    public bool IsDead => _stat[EStat.HP] <= 0;
    public EFaction FactionType { get; private set; }
    public AnimatorController AnimatorController { get; private set; } //TODO: 한 군데에서만 처리하고 싶은데.. public 으로 해야하나..
#endregion

#region Controller
    private readonly UnitAIController _unitAIController = new();
    private DeadController _deadController;
#endregion
    
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

    public void Initialize(BattleEnvironment battleEnvironment, EFaction factionType)
    {
        _battleEnvironment = battleEnvironment;
        FactionType = factionType;
        _unitUI.Initialize();
        _unitAIController.Initialize(this, battleEnvironment.Units);
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
            _battleEnvironment.RemoveUnit(this);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosUtil.DrawFOV(transform, 5f, 30f);
    }
}
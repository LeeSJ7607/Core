using R3;
using UnityEngine;
using UnityEngine.AI;

public interface IAttacker
{
    bool IsAttackable { get; set; }
    int Damage { get; }
}

public interface IDefender
{
    Transform Tm { get; }
    void Hit(int damage);
}

[RequireComponent(typeof(NavMeshAgent), typeof(AnchorNode))]
public abstract class Unit : MonoBehaviour,
    IAttacker, //TODO: 이 인터페이스를 잘 활용해보자. 필요한지 부터.
    IDefender  //TODO: 이 인터페이스를 잘 활용해보자. 필요한지 부터.
{
    public bool IsDead => _stat[EStat.HP] <= 0;
    private Stat _stat;
    private UnitUI _unitUI;
    private EFaction _factionType;
    
#region R3
    public Observable<R3.Unit> OnRelease => _onRelease;
    private readonly ReactiveCommand _onRelease = new();
#endregion

#region Attacker
    bool IAttacker.IsAttackable { get; set; }
    int IAttacker.Damage => 100;
#endregion

#region Defender
    Transform IDefender.Tm => transform;
#endregion

#region Controller
    private UnitAIController _unitAIController;
    private DeadController _deadController;
    public AnimatorController AnimatorController { get; private set; } //TODO: 한 군데에서만 처리하고 싶은데.. public 으로 해야하나..
#endregion
    
    protected virtual void Awake()
    {
        _stat = new Stat(this);
        _unitUI = new UnitUI(this);
        _unitAIController = new UnitAIController(this);
        _deadController = new DeadController(this);
        AnimatorController = new AnimatorController(this);
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

    public void Initialize(EFaction factionType)
    {
        _factionType = factionType;
        _unitUI.Initialize();
        _unitAIController.Initialize();
        _deadController.Initialize();
        AnimatorController.Initialize();
    }

    void IDefender.Hit(int damage)
    {
        _stat[EStat.HP] -= damage;
        //_uiUnit.HPAndDamage(_stat, damage_);
        AnimatorController.SetState(IsDead ? EAnimState.Die : EAnimState.Hit);
        
        // if (IsDead)
        // {
        //     SpawnedUnitContainer.Instance.Remove(this);
        // }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GizmosUtil.DrawFOV(transform, 5f, 30f);
    }
}
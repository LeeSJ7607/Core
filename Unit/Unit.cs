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

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : MonoBehaviour,
    IAttacker, //TODO: 이 인터페이스를 잘 활용해보자. 필요한지 부터.
    IDefender  //TODO: 이 인터페이스를 잘 활용해보자. 필요한지 부터.
{
    bool IAttacker.IsAttackable { get; set; }
    int IAttacker.Damage => 100;
    Transform IDefender.Tm => transform;
    public bool IsDead => _stat[EStat.HP] <= 0;
    private EFaction _factionType;
    private Stat _stat;
    private UnitAIController _unitAIController;
    private DeadController _deadController;
    public AnimatorController AnimatorController { get; private set; } //TODO: 한 군데에서만 처리하고 싶은데.. public 으로 해야하나..
    
    protected virtual void Awake()
    {
        _stat = new Stat(this);
        _unitAIController = new UnitAIController(this);
        _deadController = new DeadController(this);
        AnimatorController = new AnimatorController(GetComponent<Animator>());
    }

    private void OnDisable()
    {
        _deadController.Release();
        AnimatorController.Release();
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        _unitAIController.OnUpdate();
        AnimatorController.OnUpdate();
    }

    public void Initialize(EFaction factionType)
    {
        _factionType = factionType;
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
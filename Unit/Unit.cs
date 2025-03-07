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
    public bool IsDead => true;
    public AnimatorController AnimatorController { get; private set; } //TODO: 한 군데에서만 처리하고 싶은데.. public 으로 해야하나..
    private UnitAIController _unitAIController;

    protected virtual void Awake()
    {
        _unitAIController = new UnitAIController(this);
        AnimatorController = new AnimatorController(GetComponent<Animator>());
    }

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        _unitAIController.OnUpdate();
    }

    void IDefender.Hit(int damage)
    {
        AnimatorController.SetState(EAnimState.Hit);
    }
}
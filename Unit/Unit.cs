using UnityEngine;
using UnityEngine.AI;

public interface IAttacker
{
    bool IsAttackable { get; set; }
}

public interface IDefender
{
    void Hit(int damage);
}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Unit : MonoBehaviour,
    IAttacker,
    IDefender
{
    bool IAttacker.IsAttackable { get; set; }
    public bool IsDead => true;
    public AnimatorController AnimatorController { get; private set; }
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
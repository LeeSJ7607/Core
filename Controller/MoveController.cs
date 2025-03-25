using UnityEngine;
using UnityEngine.AI;

public enum EMoveState
{
    Moving,
    ReachedGoal,
}

public sealed class MoveController
{
    private readonly NavMeshAgent _navMeshAgent; //TODO: rcdtcs
    private readonly IAnimatorController _animatorController;
    private readonly IReadOnlyStat _stat;
    
    public MoveController(IReadOnlyUnit owner)
    {
        _navMeshAgent = owner.Tm.GetComponent<NavMeshAgent>();
        _animatorController = owner.AnimatorController;
        _stat = owner.Stat;
    }

    public void MoveTo(Vector3 targetPos)
    {
        _navMeshAgent.SetDestination(targetPos);
        GetMoveState();
    }

    public EMoveState GetMoveState()
    {
        var moveState = _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance //TODO: 0.5f는 몹끼리 부딪힐 때 계속 Moving 이라서
            ? EMoveState.Moving
            : EMoveState.ReachedGoal;
        
        SetAnimState(moveState);
        return moveState;
    }

    private void SetAnimState(EMoveState moveState)
    {
        if (moveState == EMoveState.ReachedGoal)
        {
            _animatorController.SetState(EAnimState.Idle);
            return;
        }

        _navMeshAgent.speed = _stat[EStat.WALK_SPEED];
        _animatorController.SetState(EAnimState.Walk, _navMeshAgent.speed);
    }
}
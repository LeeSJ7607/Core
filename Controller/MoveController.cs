using UnityEngine;
using UnityEngine.AI;

public enum EMoveState
{
    Moving,
    ReachedGoal,
}

public sealed class MoveController
{
    private readonly IReadOnlyStat _stat;
    private readonly IAnimatorController _animatorController;
    private readonly NavMeshAgent _navMeshAgent; //TODO: rcdtcs
    private readonly float _navMeshAgentRadius;
    
    public MoveController(IReadOnlyUnit owner)
    {
        _stat = owner.Stat;
        _animatorController = owner.AnimatorController;
        _navMeshAgent = owner.Tm.GetComponent<NavMeshAgent>();
        _navMeshAgentRadius = _navMeshAgent.radius;
    }

    public void MoveTo(Vector3 targetPos)
    {
        _navMeshAgent.SetDestination(targetPos);
        GetMoveState();
    }

    public EMoveState GetMoveState()
    {
        var stoppingDistance = _navMeshAgent.stoppingDistance + _navMeshAgentRadius;
        var moveState = _navMeshAgent.remainingDistance > stoppingDistance
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
using UnityEngine;
using UnityEngine.AI;

public enum eMoveState
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

    public eMoveState GetMoveState()
    {
        var stoppingDistance = _navMeshAgent.stoppingDistance + _navMeshAgentRadius;
        var moveState = _navMeshAgent.remainingDistance > stoppingDistance
            ? eMoveState.Moving
            : eMoveState.ReachedGoal;
        
        SetAnimState(moveState);
        return moveState;
    }

    private void SetAnimState(eMoveState moveState)
    {
        if (moveState == eMoveState.ReachedGoal)
        {
            _animatorController.SetState(eAnimState.Idle);
            return;
        }

        _navMeshAgent.speed = _stat[eStat.WALK_SPEED];
        _animatorController.SetState(eAnimState.Walk, _navMeshAgent.speed);
    }
}
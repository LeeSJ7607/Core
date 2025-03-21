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
    
    public MoveController(IReadOnlyUnit owner)
    {
        _navMeshAgent = owner.Tm.GetComponent<NavMeshAgent>();
        _animatorController = owner.AnimatorController;
    }

    public EMoveState MoveTo(Vector3 targetPos)
    {
        _navMeshAgent.SetDestination(targetPos);
        
        var moveState = _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance 
            ? EMoveState.Moving 
            : EMoveState.ReachedGoal;

        SetAnimState(moveState);
        return moveState;
    }

    private void SetAnimState(EMoveState moveState)
    {
        if (moveState == EMoveState.ReachedGoal)
        {
            _animatorController.SetState(EAnimState.Walk);
            return;
        }

        var isWalk = _navMeshAgent.remainingDistance < 2f;
        var speed = isWalk ? 1f : 3f;
        _navMeshAgent.speed = speed;
        _animatorController.SetState(EAnimState.Walk, _navMeshAgent.velocity.magnitude);
    }
}
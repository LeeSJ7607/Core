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
    private readonly AnimatorController _animatorController;
    
    public MoveController(Unit unit)
    {
        _navMeshAgent = unit.GetComponent<NavMeshAgent>();
        _animatorController = unit.AnimatorController;
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

        var isWalk = _navMeshAgent.remainingDistance < 2f; //TODO: MonsterTable.
        var speed = isWalk ? 1f : 3f; //TODO: MonsterTable.
        _animatorController.SetState(EAnimState.Walk, speed);
    }
}
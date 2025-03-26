using UnityEngine;

internal sealed class BTAction_Patrol : BehaviorTree
{
    private const float _radius = 7f; //TODO: 외부에서 값을 받아와야함.
    private Vector3 _originPos;
    private Vector3 _targetPos;

    public override void OnBegin(BlackBoard blackBoard)
    {
        base.OnBegin(blackBoard);
        _originPos = blackBoard.OwnerPos;
        SetAndMoveToNextTarget(blackBoard.MoveController);
    }

    public override EBTStatus OnUpdate(BlackBoard blackBoard)
    {
        var moveController = blackBoard.MoveController;
        if (moveController.GetMoveState() == EMoveState.ReachedGoal)
        {
            SetAndMoveToNextTarget(moveController);
        }
        
        return blackBoard.TargetController.TryFindTarget(blackBoard.Units) 
            ? EBTStatus.Success 
            : EBTStatus.Running;
    }

    private void SetAndMoveToNextTarget(MoveController moveController)
    {
        _targetPos = MathUtil.GetRandomPositionInRadius(_originPos, _radius);
        moveController.MoveTo(_targetPos);
    }
}
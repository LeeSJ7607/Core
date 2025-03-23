using UnityEngine;

internal sealed class BTAction_Patrol : BehaviorTree
{
    private const float _radius = 7f; //TODO: 외부에서 값을 받아와야함.
    private Vector3 _originPos;
    private Vector3 _targetPos;

    public override void OnBegin(BlackBoard board)
    {
        base.OnBegin(board);
        _originPos = board.OwnerPos;
        SetAndMoveToNextTarget(board.MoveController);
    }

    public override EBTStatus OnUpdate(BlackBoard board)
    {
        var moveController = board.MoveController;
        if (moveController.GetMoveState() == EMoveState.ReachedGoal)
        {
            SetAndMoveToNextTarget(moveController);
        }
        
        return board.TargetController.TryFindTarget(board.Units) 
            ? EBTStatus.Success 
            : EBTStatus.Running;
    }

    private void SetAndMoveToNextTarget(MoveController moveController)
    {
        _targetPos = MathUtil.GetRandomPositionInRadius(_originPos, _radius);
        moveController.MoveTo(_targetPos);
    }
}
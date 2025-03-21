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
        SetTargetPos();
    }

    public override EBTStatus OnUpdate(BlackBoard board)
    {
        if (board.MoveController.MoveTo(_targetPos) == EMoveState.ReachedGoal)
        {
            SetTargetPos();
        }
        
        return board.TargetController.TryFindTarget(board.Units) 
            ? EBTStatus.Success 
            : EBTStatus.Running;
    }

    private void SetTargetPos()
    {
        _targetPos = MathUtil.GetRandomPositionInRadius(_originPos, _radius);
    }
}
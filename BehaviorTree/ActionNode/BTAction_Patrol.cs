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
        
        //TODO: DrawFOV 함수에 맞춰서 시야각에 타겟이 걸렸는지 체크 필요
        return board.TargetController.TryFindTarget(board.Units) 
            ? EBTStatus.Success 
            : EBTStatus.Running;
    }

    private void SetTargetPos()
    {
        _targetPos = MathUtil.GetRandomPositionInRadius(_originPos, _radius);
    }
}
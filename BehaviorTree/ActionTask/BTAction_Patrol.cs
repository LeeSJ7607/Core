using UnityEngine;

internal sealed class BTAction_Patrol : BTNode
{
    private const float _radius = 3f; //TODO: 외부에서 값을 받아와야함.
    private Vector3 _originPos;
    private Vector3 _targetPos;

    public override void OnBegin(BTBoard board)
    {
        _originPos = board.OwnerPos;
        SetTargetPos();
    }

    public override EBTStatus OnUpdate(BTBoard board)
    {
        if (board.MoveController.MoveTo(_targetPos) == EMoveState.ReachedGoal)
        {
            SetTargetPos();
        }

        return EBTStatus.Success;

    }

    private void SetTargetPos()
    {
        _targetPos = MathUtil.GetRandomPositionInRadius(_originPos, _radius);
    }
}
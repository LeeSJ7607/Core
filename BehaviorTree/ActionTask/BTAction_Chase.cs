using UnityEngine;

internal sealed class BTAction_Chase : BTNode
{
    public override EBTStatus OnUpdate(BTBoard board)
    {
        var target = board.Target;
        var targetPos = target.transform.position;
        board.MoveController.MoveTo(targetPos);

        if (target.IsDead())
        {
            return EBTStatus.Failure;
        }
        
        if (IsTargetTooFar(board.OwnerPos, targetPos))
        {
            return EBTStatus.Failure;
        }
        
        return board.AttackController.IsTargetInRange(targetPos)
            ? EBTStatus.Success
            : EBTStatus.Running;
    }

    private bool IsTargetTooFar(Vector3 ownerPos, Vector3 targetPos)
    {
        return Vector3.Distance(ownerPos, targetPos) > 5f; //TODO: 타겟이 너무 멀면 추적 취소 (테이블 필요)
    }
}
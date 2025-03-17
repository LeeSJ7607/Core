using UnityEngine;

internal sealed class BTAction_Chase : BTNode
{
    private const float _maxChaseDistance = 5f;
    
    public override EBTStatus OnUpdate(BTBoard board)
    {
        var target = board.Target;
        var targetPos = target.Tm.position;
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
        return Vector3.Distance(ownerPos, targetPos) > _maxChaseDistance;
    }
}
using UnityEngine;

internal sealed class BTAction_Chase : BehaviorTree
{
    private const float _maxChaseDistance = 5f;
    
    public override EBTStatus OnUpdate(BlackBoard blackBoard)
    {
        var target = blackBoard.Target;
        var targetPos = target.Tm.position;
        blackBoard.MoveController.MoveTo(targetPos);

        if (target.IsDead())
        {
            return EBTStatus.Failure;
        }
        
        if (IsTargetTooFar(blackBoard.OwnerPos, targetPos))
        {
            return EBTStatus.Failure;
        }
        
        return blackBoard.AttackController.IsTargetInRange(targetPos)
            ? EBTStatus.Success
            : EBTStatus.Running;
    }

    private bool IsTargetTooFar(Vector3 ownerPos, Vector3 targetPos)
    {
        return targetPos.SqrDistance(ownerPos) > _maxChaseDistance.Sqr();
    }
}
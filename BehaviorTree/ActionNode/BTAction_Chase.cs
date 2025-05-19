using UnityEngine;

internal sealed class BTAction_Chase : BehaviorTree
{
    private const float _maxChaseDistance = 5f;
    
    public override eBTStatus OnUpdate(BlackBoard blackBoard)
    {
        var target = blackBoard.Target;
        var targetPos = target.Tm.position;
        blackBoard.MoveController.MoveTo(targetPos);

        if (target.IsDead())
        {
            return eBTStatus.Failure;
        }
        
        if (IsTargetTooFar(blackBoard.OwnerPos, targetPos))
        {
            return eBTStatus.Failure;
        }
        
        return blackBoard.AttackController.IsTargetInRange(targetPos)
            ? eBTStatus.Success
            : eBTStatus.Running;
    }

    private bool IsTargetTooFar(Vector3 ownerPos, Vector3 targetPos)
    {
        return targetPos.SqrDistance(ownerPos) > _maxChaseDistance.Sqr();
    }
}
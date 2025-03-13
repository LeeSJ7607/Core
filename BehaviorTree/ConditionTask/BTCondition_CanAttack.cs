public sealed class BTCondition_CanAttack : BTNode
{
    public override EBTStatus OnUpdate(BTBoard board)
    {
        var target = board.Target;
        if (target.IsDead())
        {
            return EBTStatus.Failure;
        }
        
        return board.AttackController.IsTargetInRange(target.transform.position)
            ? EBTStatus.Success
            : EBTStatus.Failure;
    }
}
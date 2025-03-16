internal sealed class BTAction_Attack : BTNode
{
    public override EBTStatus OnUpdate(BTBoard board)
    {
        var target = board.TargetController.Target;
        var attackController = board.AttackController;
        attackController.Attack(target);
        
        if (!attackController.IsTargetInRange(target.transform.position))
        {
            return EBTStatus.Failure;
        }
        
        return target.IsDead()
            ? EBTStatus.Success
            : EBTStatus.Running;
    }
}
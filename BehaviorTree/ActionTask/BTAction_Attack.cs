internal sealed class BTAction_Attack : BTNode
{
    public override EBTStatus OnUpdate(BTBoard board)
    {
        var target = board.TargetController.Target;
        if (target.IsDead)
        {
            return EBTStatus.Success;
        }

        var attackController = board.AttackController;
        if (!attackController.IsTargetInRange(target.transform.position))
        {
            return EBTStatus.Failure;
        }
        
        attackController.Attack(target);
        return EBTStatus.Running;
    }
}
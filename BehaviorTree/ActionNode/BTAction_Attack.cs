internal sealed class BTAction_Attack : BehaviorTree
{
    public override EBTStatus OnUpdate(BlackBoard board)
    {
        var target = board.TargetController.Target;
        var attackController = board.AttackController;
        attackController.Attack((IDefender)target);
        
        if (!attackController.IsTargetInRange(target.Tm.position))
        {
            return EBTStatus.Failure;
        }
        
        return target.IsDead()
            ? EBTStatus.Success
            : EBTStatus.Running;
    }
}
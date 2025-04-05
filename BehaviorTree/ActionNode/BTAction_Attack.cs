internal sealed class BTAction_Attack : BehaviorTree
{
    private IDefender _target;
    
    public override EBTStatus OnUpdate(BlackBoard blackBoard)
    {
        var target = blackBoard.TargetController.Target;
        _target ??= (IDefender)target;
        
        var attackController = blackBoard.AttackController;
        attackController.Attack(_target);
        
        if (!attackController.IsTargetInRange(target.Tm.position))
        {
            return EBTStatus.Failure;
        }
        
        return target.IsDead()
            ? EBTStatus.Success
            : EBTStatus.Running;
    }

    public override void OnEnd(BlackBoard blackBoard)
    {
        _target = null;
        base.OnEnd(blackBoard);
    }
}
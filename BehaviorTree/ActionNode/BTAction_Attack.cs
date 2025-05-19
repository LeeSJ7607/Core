internal sealed class BTAction_Attack : BehaviorTree
{
    private IDefender _target;
    
    public override eBTStatus OnUpdate(BlackBoard blackBoard)
    {
        var target = blackBoard.TargetController.Target;
        _target ??= (IDefender)target;
        
        var attackController = blackBoard.AttackController;
        attackController.Attack(_target);
        
        if (!attackController.IsTargetInRange(target.Tm.position))
        {
            return eBTStatus.Failure;
        }
        
        return target.IsDead()
            ? eBTStatus.Success
            : eBTStatus.Running;
    }

    public override void OnEnd(BlackBoard blackBoard)
    {
        _target = null;
        base.OnEnd(blackBoard);
    }
}
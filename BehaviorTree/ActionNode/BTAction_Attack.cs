internal sealed class BTAction_Attack : BehaviorTree
{
    private IDefender _target;
    
    public override EBTStatus OnUpdate(BlackBoard board)
    {
        var target = board.TargetController.Target;
        _target ??= (IDefender)target;
        
        var targetPos = target.Tm.position;
        board.MoveController.MoveTo(targetPos);
        
        var attackController = board.AttackController;
        attackController.Attack(_target);
        
        if (!attackController.IsTargetInRange(targetPos))
        {
            return EBTStatus.Failure;
        }
        
        return target.IsDead()
            ? EBTStatus.Success
            : EBTStatus.Running;
    }

    public override void OnEnd(BlackBoard board)
    {
        _target = null;
        base.OnEnd(board);
    }
}
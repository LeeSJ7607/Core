internal sealed class BTAction_Chase : BTNode
{
    public override EBTStatus OnUpdate(BTBoard board)
    {
        var target = board.TargetController.Target;
        if (target.IsDead())
        {
            return EBTStatus.Failure;
        }

        if (board.AttackController.IsTargetInRange(target.transform.position))
        {
            return EBTStatus.Success;
        }
        
        return EBTStatus.Running;
    }
}
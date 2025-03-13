internal sealed class BTAction_Chase : BTNode
{
    public override EBTStatus OnUpdate(BTBoard board)
    {
        var target = board.Target;
        if (target.IsDead())
        {
            return EBTStatus.Failure;
        }

        board.MoveController.MoveTo(board.Target.transform.position);
        return EBTStatus.Success;
    }
}
public sealed class BTCondition_TargetSearch : BTNode
{
    public override EBTStatus OnUpdate(BTBoard board)
    {
        return board.TargetController.TryFindTarget() 
            ? EBTStatus.Success
            : EBTStatus.Failure;
    }
}
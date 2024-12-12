internal sealed class BTAction_Chase : BTNode
{
    public override Status Update()
    {
        return Status.Running;
    }
}
internal sealed class BTSelector : BTComposite
{
    public override EBTStatus Update()
    {
        return Process(EBTStatus.Failure);
    }
}
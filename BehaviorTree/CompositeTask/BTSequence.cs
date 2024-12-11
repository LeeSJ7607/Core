internal sealed class BTSequence : BTComposite
{
    public override EBTStatus Update()
    {
        return Process(EBTStatus.Success);
    }
}
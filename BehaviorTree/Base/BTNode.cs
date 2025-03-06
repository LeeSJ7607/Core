public abstract class BTNode
{
    public virtual void OnBegin(BTBoard board) { }
    public abstract EBTStatus OnUpdate(BTBoard board);
    public virtual void OnEnd(BTBoard board) { }
}
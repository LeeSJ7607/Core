public abstract class BTNode
{
    public enum Status
    {
        Success,
        Failure,
        Running
    }
    
    public abstract Status Update();
    public virtual void End() { }
}
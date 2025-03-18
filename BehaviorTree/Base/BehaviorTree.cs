public enum EBTStatus
{
    Success,
    Failure,
    Running
}

public abstract class BehaviorTree
{
    public virtual void OnBegin(BlackBoard board) { }
    public abstract EBTStatus OnUpdate(BlackBoard board);
    public virtual void OnEnd(BlackBoard board) { }
}
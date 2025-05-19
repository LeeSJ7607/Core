public enum eBTStatus
{
    Success,
    Failure,
    Running
}

public abstract class BehaviorTree
{
    public virtual void OnBegin(BlackBoard blackBoard) { }
    public abstract eBTStatus OnUpdate(BlackBoard blackBoard);
    public virtual void OnEnd(BlackBoard blackBoard) { }
}
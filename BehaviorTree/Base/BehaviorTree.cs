public enum EBTStatus
{
    Success,
    Failure,
    Running
}

public abstract class BehaviorTree
{
    public virtual void OnBegin(BlackBoard blackBoard) { }
    public abstract EBTStatus OnUpdate(BlackBoard blackBoard);
    public virtual void OnEnd(BlackBoard blackBoard) { }
}
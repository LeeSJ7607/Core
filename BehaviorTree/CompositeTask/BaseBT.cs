internal abstract class BaseBT
{
    public abstract EBTStatus Update();
    public virtual void Begin() { }
    public virtual void End() { }
}
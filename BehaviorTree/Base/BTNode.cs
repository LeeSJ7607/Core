using UnityEngine;

//TODO: ScriptableObject 왜 상속 받지?
public abstract class BTNode : ScriptableObject
{
    public enum Status
    {
        Success,
        Failure,
        Running
    }
    
    public abstract Status Update();
    public virtual void Begin() { }
    public virtual void End() { }
}
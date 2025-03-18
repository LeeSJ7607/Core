using UnityEngine;

public abstract class HierachyTree : MonoBehaviour
{
    public virtual void OnBegin() { }
    public abstract EBTStatus OnUpdate();
    public virtual void OnEnd() { }
}
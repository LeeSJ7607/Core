using UnityEngine;

public abstract class HierarchyTree : MonoBehaviour
{
    public virtual void OnBegin() { }
    public abstract EBTStatus OnUpdate();
    public virtual void OnEnd() { }
}
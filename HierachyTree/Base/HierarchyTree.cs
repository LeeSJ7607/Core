using UnityEngine;

public abstract class HierarchyTree : MonoBehaviour
{
    public virtual void OnBegin() { }
    public abstract eBTStatus OnUpdate();
    public virtual void OnEnd() { }
}
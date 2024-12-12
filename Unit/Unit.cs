using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    private IBTComposite _btRoot;
    
    protected virtual void Awake()
    {
        _btRoot = new BTSelector();
        _btRoot.AddTask(new BTAction_Attack())
               .AddTask(new BTAction_Chase());
    }
    
    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        _btRoot.UpdateTask();
    }
}
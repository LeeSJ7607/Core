using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    private IBTComposite _btRoot;
    
    protected virtual void Awake()
    {
        _btRoot = new BTSequence();
        _btRoot.AddTask(new List<BaseBT>()
        {
            new BTAction_Attack(),
            new BTAction_Chase(),
        });
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
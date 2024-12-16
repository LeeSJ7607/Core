using System;
using GraphViewBehaviorTree.Nodes;
using MoreMountains.Tools;
using UnityEngine;

//TODO: 리버트 필수.
public class Unit : MonoBehaviour
{
    private BehaviorTree _behaviorTree;
    
    protected virtual void Awake()
    {
        _behaviorTree = ScriptableObject.CreateInstance<BehaviorTree>();
        var d = ScriptableObject.CreateInstance<DebugLogNode>();
        _behaviorTree.RootNode = d;
        _behaviorTree.TreeStatus = BTNode.Status.Running;
    }

    private void Update()
    {
        if (_behaviorTree != null)
        {
            _behaviorTree.Update();
        }
    }
}
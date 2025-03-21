using System.Collections.Generic;
using UnityEngine;

public interface IHTComposite
{
    EBTStatus Update();
}

public abstract class HTComposite : HierachyTree, IHTComposite
{
    protected readonly List<HierachyTree> _nodes = new();

    private void Awake()
    {
        var tm = transform;
        foreach (var obj in tm)
        {
            var child = (Transform)obj;
            if (child.TryGetComponent<HierachyTree>(out var node))
            {
                _nodes.Add(node);
            }
        }

        if (_nodes.IsNullOrEmpty())
        {
            Debug.LogError($"컴포넌트된 {nameof(HierachyTree)}가 없습니다.");
        }
    }

    EBTStatus IHTComposite.Update()
    {
        if (_nodes.IsNullOrEmpty())
        {
            return EBTStatus.Failure;
        }
        
        return OnUpdate();
    }
}
using System.Collections.Generic;
using UnityEngine;

public interface IHTComposite
{
    void Release();
    eBTStatus Update();
}

public abstract class HTComposite : HierarchyTree, IHTComposite
{
    protected readonly List<HierarchyTree> _nodes = new();

    void IHTComposite.Release()
    {
        _nodes.Clear();
    }

    private void Awake()
    {
        var tm = transform;
        foreach (var obj in tm)
        {
            var child = (Transform)obj;
            if (child.TryGetComponent<HierarchyTree>(out var node))
            {
                _nodes.Add(node);
            }
        }

        if (_nodes.IsNullOrEmpty())
        {
            Debug.LogError($"컴포넌트된 {nameof(HierarchyTree)}가 없습니다.");
        }
    }

    eBTStatus IHTComposite.Update()
    {
        if (_nodes.IsNullOrEmpty())
        {
            return eBTStatus.Failure;
        }
        
        return OnUpdate();
    }
}
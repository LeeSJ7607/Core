using System.Collections.Generic;

internal interface IBTComposite
{
    IBTComposite AddNode(BehaviorTree node);
    void Update(BlackBoard board);
}

internal abstract class BTComposite : BehaviorTree, IBTComposite
{
    protected readonly List<BehaviorTree> _nodes = new();

    IBTComposite IBTComposite.AddNode(BehaviorTree node)
    {
        _nodes.Add(node);
        return this;
    }

    void IBTComposite.Update(BlackBoard board)
    {
        if (_nodes.IsNullOrEmpty())
        {
            return;
        }
        
        OnUpdate(board);
    }
}
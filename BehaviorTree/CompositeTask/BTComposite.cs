using System.Collections.Generic;

internal interface IBTComposite
{
    IBTComposite AddNode(BTNode node);
    void Update(BTBoard board);
}

internal abstract class BTComposite : BTNode, IBTComposite
{
    protected readonly List<BTNode> _nodes = new();

    IBTComposite IBTComposite.AddNode(BTNode node)
    {
        _nodes.Add(node);
        return this;
    }

    void IBTComposite.Update(BTBoard board)
    {
        if (_nodes.IsNullOrEmpty())
        {
            return;
        }
        
        OnUpdate(board);
    }
}
using System.Collections.Generic;

//TODO: 필요한 인터페이스인가??
internal interface IBTComposite
{
    IBTComposite AddNode(BTNode node);
    void Execute();
}

internal abstract class BTComposite : BTNode, IBTComposite
{
    protected readonly List<BTNode> _nodes = new();
    
    IBTComposite IBTComposite.AddNode(BTNode node)
    {
        _nodes.Add(node);
        return this;
    }

    void IBTComposite.Execute()
    {
        if (!_nodes.NullOrEmpty())
        {
            Update();
        }
    }
}
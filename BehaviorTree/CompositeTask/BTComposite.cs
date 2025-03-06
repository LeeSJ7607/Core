using System.Collections.Generic;

internal interface IBTComposite
{
    IBTComposite AddNode(BTNode node);
    EBTStatus? Update(BTBoard board);
}

internal abstract class BTComposite : BTNode, IBTComposite
{
    protected readonly List<BTNode> _nodes = new();

    IBTComposite IBTComposite.AddNode(BTNode node)
    {
        _nodes.Add(node);
        return this;
    }

    EBTStatus? IBTComposite.Update(BTBoard board)
    {
        if (_nodes.IsNullOrEmpty())
        {
            return null;
        }
        
        return OnUpdate(board);
    }
}
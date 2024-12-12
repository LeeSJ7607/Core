using System.Collections.Generic;

internal interface IBTComposite
{
    IBTComposite AddTask(BTNode node);
    void UpdateTask();
}

internal abstract class BTComposite : BTNode, IBTComposite
{
    protected readonly List<BTNode> _nodes = new();
    
    IBTComposite IBTComposite.AddTask(BTNode node)
    {
        _nodes.Add(node);
        return this;
    }

    void IBTComposite.UpdateTask()
    {
        if (_nodes.NullOrEmpty())
        {
            return;
        }
        
        Update();
    }
}
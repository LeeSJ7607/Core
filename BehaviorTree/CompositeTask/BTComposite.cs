using System.Collections.Generic;

internal interface IBTComposite
{
    IBTComposite AddTask(BaseBT tasks);
    void UpdateTask();
}

internal abstract class BTComposite : BaseBT, IBTComposite
{
    protected readonly List<BaseBT> _tasks = new();
    
    IBTComposite IBTComposite.AddTask(BaseBT task)
    {
        _tasks.Add(task);
        return this;
    }

    void IBTComposite.UpdateTask()
    {
        if (_tasks.NullOrEmpty())
        {
            return;
        }
        
        Update();
    }
}
using System.Collections.Generic;

internal interface IBTComposite
{
    void AddTask(IReadOnlyList<BaseBT> tasks);
    void UpdateTask();
}

internal abstract class BTComposite : BaseBT, IBTComposite
{
    protected readonly List<BaseBT> _tasks = new();
    
    void IBTComposite.AddTask(IReadOnlyList<BaseBT> tasks)
    {
        _tasks.AddRange(tasks);
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
using System.Collections.Generic;

internal interface IBTComposite
{
    void AddTask(IReadOnlyList<BaseBT> tasks);
    void UpdateTask();
}

internal abstract class BTComposite : BaseBT, IBTComposite
{
    private int _curTaskIdx;
    private EBTStatus? _curBtStatus;
    private readonly List<BaseBT> _tasks = new();
    
    private void Reset()
    {
        _curTaskIdx = 0;
        _curBtStatus = null;
    }

    void IBTComposite.AddTask(IReadOnlyList<BaseBT> tasks)
    {
        _tasks.AddRange(tasks);
    }

    void IBTComposite.UpdateTask()
    {
        if (Update() != EBTStatus.Running)
        {
            Reset();
        }
    }

    protected EBTStatus Process(EBTStatus btStatus)
    {
        while (_curTaskIdx < _tasks.Count)
        {
            var curTask = _tasks[_curTaskIdx];

            if (IsNotRunning())
            {
                curTask.Begin();
            }

            _curBtStatus = curTask.Update();
            
            if (IsNotRunning())
            {
                curTask.End();
            }
        }

        return EBTStatus.Success;
    }

    private bool IsNotRunning()
    {
        return _curBtStatus is not EBTStatus.Running;
    }
}
internal class BTSelector : BTComposite
{
    protected int _curTaskIdx;
    
    protected virtual void ResetTaskIdx()
    {
        _curTaskIdx = 0;
    }
    
    protected virtual void MoveToNextTask()
    {
        ++_curTaskIdx;
    }
    
    public override EBTStatus Update()
    {
        if (_curTaskIdx == _tasks.Count)
        {
            ResetTaskIdx();
            return EBTStatus.Success;
        }

        var curTask = _tasks[_curTaskIdx];
        var status = curTask.Update();
        
        if (status == EBTStatus.Running)
        {
            return EBTStatus.Running;
        }
        
        if (status == EBTStatus.Success)
        {
            ResetTaskIdx();
            return EBTStatus.Success;
        }
        
        curTask.End();
        MoveToNextTask();
        return EBTStatus.Running;
    }
}
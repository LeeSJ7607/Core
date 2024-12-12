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
    
    public override Status Update()
    {
        if (_curTaskIdx == _nodes.Count)
        {
            ResetTaskIdx();
            return Status.Success;
        }

        var curTask = _nodes[_curTaskIdx];
        var status = curTask.Update();
        
        if (status == Status.Running)
        {
            return Status.Running;
        }
        
        if (status == Status.Success)
        {
            ResetTaskIdx();
            return Status.Success;
        }
        
        curTask.End();
        MoveToNextTask();
        return Status.Running;
    }
}
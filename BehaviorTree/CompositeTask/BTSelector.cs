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
        ResetTaskIdx();

        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            var status = curTask.Update();

            if (status == Status.Running)
            {
                continue;
            }

            if (status == Status.Success)
            {
                ResetTaskIdx();
                return Status.Success;
            }

            curTask.End();
            MoveToNextTask();
        }

        return Status.Failure;
    }
}
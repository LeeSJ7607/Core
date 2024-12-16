internal class BTSelector : BTComposite
{
    protected bool _started;
    protected int _curTaskIdx;
    
    protected virtual void ResetTaskIdx()
    {
        _started = false;
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

            if (!_started)
            {
                curTask.Begin();
                _started = true;
            }
            
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
            _started = false;
            MoveToNextTask();
        }

        return Status.Failure;
    }
}
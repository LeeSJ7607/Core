internal sealed class BTSequence : BTComposite
{
    private bool _started;
    private int _curTaskIdx;

    private void ResetTaskIdx()
    {
        _started = false;
        _curTaskIdx = 0;
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

            if (status == Status.Failure)
            {
                ResetTaskIdx();
                continue;
            }

            curTask.End();
            _started = false;
            ++_curTaskIdx;
        }

        return Status.Success;
    }
}
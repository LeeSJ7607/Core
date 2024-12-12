internal sealed class BTSequence : BTComposite
{
    private int _curTaskIdx;

    private void ResetTaskIdx()
    {
        _curTaskIdx = 0;
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

        if (status == Status.Failure)
        {
            ResetTaskIdx();
            return Status.Failure;
        }

        curTask.End();
        ++_curTaskIdx;
        return Status.Running;
    }
}
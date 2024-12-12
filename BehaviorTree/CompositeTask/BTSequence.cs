internal sealed class BTSequence : BTComposite
{
    private int _curTaskIdx;

    private void ResetTaskIdx()
    {
        _curTaskIdx = 0;
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

            if (status == Status.Failure)
            {
                ResetTaskIdx();
                continue;
            }

            curTask.End();
            ++_curTaskIdx;
        }

        return Status.Success;
    }
}
internal sealed class BTSequence : BTComposite
{
    private int _curTaskIdx;

    private void ResetTaskIdx()
    {
        _curTaskIdx = 0;
    }
    
    public override EBTStatus Update()
    {
        if (_curTaskIdx == _nodes.Count)
        {
            ResetTaskIdx();
            return EBTStatus.Success;
        }

        var curTask = _nodes[_curTaskIdx];
        var status = curTask.Update();
        
        if (status == EBTStatus.Running)
        {
            return EBTStatus.Running;
        }

        if (status == EBTStatus.Failure)
        {
            ResetTaskIdx();
            return EBTStatus.Failure;
        }

        curTask.End();
        ++_curTaskIdx;
        return EBTStatus.Running;
    }
}
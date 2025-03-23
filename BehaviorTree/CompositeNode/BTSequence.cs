internal sealed class BTSequence : BTComposite
{
    private int _curTaskIdx;
    private bool _canBegin = true;

    private void Reset()
    {
        _curTaskIdx = 0;
    }

    public override EBTStatus OnUpdate(BlackBoard board)
    {
        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            
            if (_canBegin)
            {
                _canBegin = false;
                curTask.OnBegin(board);
            }
            
            var curStatus = curTask.OnUpdate(board);
            if (curStatus == EBTStatus.Running)
            {
                return EBTStatus.Running;
            }

            curTask.OnEnd(board);
            ++_curTaskIdx;

            if (curStatus == EBTStatus.Failure)
            {
                Reset();
                return EBTStatus.Failure;
            }
        }
        
        Reset();
        return EBTStatus.Success;
    }
}
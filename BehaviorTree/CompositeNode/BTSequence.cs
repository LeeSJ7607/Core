internal sealed class BTSequence : BTComposite
{
    private int _curTaskIdx;
    private bool _canBegin = true;

    private void Reset()
    {
        _curTaskIdx = 0;
        _canBegin = true;
    }
    
    private void MoveToNextTask()
    {
        ++_curTaskIdx;
        _canBegin = true;
    }

    public override eBTStatus OnUpdate(BlackBoard blackBoard)
    {
        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            
            if (_canBegin)
            {
                curTask.OnBegin(blackBoard);
                _canBegin = false;
            }

            switch (curTask.OnUpdate(blackBoard))
            {
            case eBTStatus.Running: 
                return eBTStatus.Running;

            case eBTStatus.Success:
                {
                    curTask.OnEnd(blackBoard);
                    MoveToNextTask();
                }
                break;

            case eBTStatus.Failure:
                {
                    curTask.OnEnd(blackBoard);
                    Reset();
                }
                return eBTStatus.Failure;
            }
        }

        Reset();
        return eBTStatus.Success;
    }
}
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

    public override EBTStatus OnUpdate(BlackBoard blackBoard)
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
            case EBTStatus.Running: 
                return EBTStatus.Running;

            case EBTStatus.Success:
                {
                    curTask.OnEnd(blackBoard);
                    MoveToNextTask();
                }
                break;

            case EBTStatus.Failure:
                {
                    curTask.OnEnd(blackBoard);
                    Reset();
                }
                return EBTStatus.Failure;
            }
        }

        Reset();
        return EBTStatus.Success;
    }
}
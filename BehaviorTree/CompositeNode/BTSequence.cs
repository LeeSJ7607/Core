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

    public override EBTStatus OnUpdate(BlackBoard board)
    {
        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            
            if (_canBegin)
            {
                curTask.OnBegin(board);
                _canBegin = false;
            }

            switch (curTask.OnUpdate(board))
            {
            case EBTStatus.Running: 
                return EBTStatus.Running;

            case EBTStatus.Success:
                {
                    curTask.OnEnd(board);
                    MoveToNextTask();
                }
                break;

            case EBTStatus.Failure:
                {
                    curTask.OnEnd(board);
                    Reset();
                }
                return EBTStatus.Failure;
            }
        }

        Reset();
        return EBTStatus.Success;
    }
}
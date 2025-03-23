internal class BTSelector : BTComposite
{
    protected int _curTaskIdx;
    private bool _canBegin;

    protected virtual void Reset()
    {
        _curTaskIdx = 0;
        _canBegin = true;
    }

    protected virtual void MoveToNextTask()
    {
        ++_curTaskIdx;
        _canBegin = true;
    }
    
    public override EBTStatus OnUpdate(BlackBoard board)
    {
        Reset();
        
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
                }
                return EBTStatus.Success;
                
            case EBTStatus.Failure:
                {
                    curTask.OnEnd(board);
                    MoveToNextTask();
                    _canBegin = true;        
                }
                break;
            }
        }

        return EBTStatus.Failure;
    }
}
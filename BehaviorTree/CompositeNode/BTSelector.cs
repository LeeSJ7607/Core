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
    
    public override eBTStatus OnUpdate(BlackBoard blackBoard)
    {
        Reset();
        
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
                }
                return eBTStatus.Success;
                
            case eBTStatus.Failure:
                {
                    curTask.OnEnd(blackBoard);
                    MoveToNextTask();
                    _canBegin = true;        
                }
                break;
            }
        }

        return eBTStatus.Failure;
    }
}
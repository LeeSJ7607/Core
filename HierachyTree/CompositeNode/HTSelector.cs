public class HTSelector : HTComposite
{
    protected int _curTaskIdx;
    private bool _canBegin;
    
    protected virtual void ResetTree()
    {
        _curTaskIdx = 0;
        _canBegin = true;
    }
    
    protected virtual void MoveToNextTask()
    {
        ++_curTaskIdx;
        _canBegin = true;
    }
    
    public override EBTStatus OnUpdate()
    {
        ResetTree();
        
        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            
            if (_canBegin)
            {
                curTask.OnBegin();
                _canBegin = false;
            }

            switch (curTask.OnUpdate())
            {
            case EBTStatus.Running:
                return EBTStatus.Running;
            
            case EBTStatus.Success:
                {
                    curTask.OnEnd();
                }
                return EBTStatus.Success;
                
            case EBTStatus.Failure:
                {
                    curTask.OnEnd();
                    MoveToNextTask();
                    _canBegin = true;        
                }
                break;
            }
        }

        return EBTStatus.Failure;
    }
}
public class HTSelector : HTComposite
{
    protected int _curTaskIdx;
    private bool _canBegin = true;
    
    protected virtual void Reset()
    {
        _curTaskIdx = 0;
    }
    
    protected virtual void MoveToNextTask()
    {
        ++_curTaskIdx;
    }
    
    public override EBTStatus OnUpdate()
    {
        Reset();
        
        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            
            if (_canBegin)
            {
                _canBegin = false;
                curTask.OnBegin();
            }
            
            var curStatus = curTask.OnUpdate();
            if (curStatus == EBTStatus.Running)
            {
                return EBTStatus.Running;
            }

            curTask.OnEnd();
            MoveToNextTask();

            if (curStatus == EBTStatus.Success)
            {
                return EBTStatus.Success;
            }
        }

        return EBTStatus.Failure;
    }
}
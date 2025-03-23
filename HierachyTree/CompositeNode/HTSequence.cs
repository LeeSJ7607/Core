public sealed class HTSequence : HTComposite
{
    private int _curTaskIdx;
    private bool _canBegin = true;

    private void ResetTree()
    {
        _curTaskIdx = 0;
        _canBegin = true;
    }
    
    private void MoveToNextTask()
    {
        ++_curTaskIdx;
        _canBegin = true;
    }

    public override EBTStatus OnUpdate()
    {
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
                    MoveToNextTask();
                }
                break;

            case EBTStatus.Failure:
                {
                    curTask.OnEnd();
                    ResetTree();
                }
                return EBTStatus.Failure;
            }
        }

        ResetTree();
        return EBTStatus.Success;
    }
}
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

    public override eBTStatus OnUpdate()
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
            case eBTStatus.Running: 
                return eBTStatus.Running;

            case eBTStatus.Success:
                {
                    curTask.OnEnd();
                    MoveToNextTask();
                }
                break;

            case eBTStatus.Failure:
                {
                    curTask.OnEnd();
                    ResetTree();
                }
                return eBTStatus.Failure;
            }
        }

        ResetTree();
        return eBTStatus.Success;
    }
}
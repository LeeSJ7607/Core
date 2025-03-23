public sealed class HTSequence : HTComposite
{
    private int _curTaskIdx;
    private bool _canBegin = true;

    public override EBTStatus OnUpdate()
    {
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
            ++_curTaskIdx;

            if (curStatus == EBTStatus.Failure)
            {
                return EBTStatus.Failure;
            }
        }

        return EBTStatus.Success;
    }
}
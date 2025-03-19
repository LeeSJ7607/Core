using UnityEngine;

public sealed class HTSequence : HTComposite
{
    private EBTStatus? _curStatus;
    private int _curTaskIdx;

    public override EBTStatus OnUpdate()
    {
        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            Debug.Log($"Sequence: {curTask.GetType().Name}");
            
            if (_curStatus == null)
            {
                curTask.OnBegin();
            }
            
            _curStatus = curTask.OnUpdate();

            if (_curStatus == EBTStatus.Running)
            {
                return EBTStatus.Running;
            }

            curTask.OnEnd();
            ++_curTaskIdx;

            if (_curStatus == EBTStatus.Failure)
            {
                return EBTStatus.Failure;
            }
        }

        return EBTStatus.Success;
    }
}
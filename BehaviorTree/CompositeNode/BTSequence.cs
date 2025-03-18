using UnityEngine;

internal sealed class BTSequence : BTComposite
{
    private EBTStatus? _curStatus;
    private int _curTaskIdx;

    private void Reset()
    {
        _curStatus = null;
        _curTaskIdx = 0;
    }
    
    public override EBTStatus OnUpdate(BlackBoard board)
    {
        while (_curTaskIdx < _nodes.Count)
        {
            var curTask = _nodes[_curTaskIdx];
            Debug.Log($"Sequence: {curTask.GetType().Name}");
            
            if (_curStatus == null)
            {
                curTask.OnBegin(board);
            }
            
            _curStatus = curTask.OnUpdate(board);

            if (_curStatus == EBTStatus.Running)
            {
                return EBTStatus.Running;
            }

            curTask.OnEnd(board);
            ++_curTaskIdx;

            if (_curStatus == EBTStatus.Failure)
            {
                Reset();
                return EBTStatus.Failure;
            }
        }
        
        Reset();
        return EBTStatus.Success;
    }
}
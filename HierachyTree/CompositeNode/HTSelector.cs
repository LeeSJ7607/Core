using UnityEngine;

public class HTSelector : HTComposite
{
    private EBTStatus? _curStatus;
    protected int _curTaskIdx;
    
    protected virtual void Reset()
    {
        _curStatus = null;
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
            Debug.Log($"Selector: {curTask.GetType().Name}");
            
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
            MoveToNextTask();

            if (_curStatus.Value == EBTStatus.Success)
            {
                return EBTStatus.Success;
            }
        }

        return EBTStatus.Failure;
    }
}
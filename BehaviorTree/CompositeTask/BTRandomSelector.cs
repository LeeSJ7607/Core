using System.Collections.Generic;

internal sealed class BTRandomSelector : BTSelector
{
    private readonly HashSet<int> _taskIdxList = new();
    
    protected override void ResetTaskIdx()
    {
        _taskIdxList.Clear();
        _started = false;
        _curTaskIdx = RandomTaskIdx();
    }

    protected override void MoveToNextTask()
    {
        if (_taskIdxList.Count == _nodes.Count)
        {
            return;
        }
        
        var selectedTaskIdx = RandomTaskIdx();
        while (_taskIdxList.Contains(selectedTaskIdx))
        {
            selectedTaskIdx = RandomTaskIdx();
        }

        _curTaskIdx = selectedTaskIdx;
        _taskIdxList.Add(selectedTaskIdx);
    }

    private int RandomTaskIdx()
    {
        return UnityEngine.Random.Range(0, _nodes.Count);
    }
}
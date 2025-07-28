using UnityEngine;

internal sealed class PullingEffect : Effect
{
    private IReadOnlyUnit _owner;
    private IReadOnlyUnit _target;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private float _duration;
    
    protected override void DeActivate()
    {
        _owner.SetMovementAllowed(true);
        _owner = null;
        base.DeActivate();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (_target != null)
        {
            MoveTarget();
        }
    }

    protected override void ApplyToTarget(IAttacker owner, IDefender target)
    {
        _owner = (IReadOnlyUnit)owner;
        _owner.SetMovementAllowed(false);
        _target = (IReadOnlyUnit)target;
        
        _startPos = _target.Tm.position;
        _endPos = _owner.Tm.position;
        _duration = _effectTable.Duration;
    }
    
    private void MoveTarget()
    {
        _elapsedTime += Time.deltaTime;
        
        var lerpTime = Mathf.Clamp01(_elapsedTime / _duration);
        _target.Tm.position = Vector3.Lerp(_startPos, _endPos, lerpTime);
    }
}
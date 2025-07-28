using UnityEngine;

public abstract class Buff
{
    public BuffTable.Row BuffTable { get; private set; }
    private IReadOnlyUnit _target;
    private int _value;
    private float _elapsedTime;
    private int _stackCount = 1;
    
    protected virtual void DeActivate()
    {
        _target.RemoveBuffFlags(BuffTable.BuffEffectType);
        _target = null;
    }
    
    public virtual void Apply(BuffTable.Row buffTable, IReadOnlyUnit target)
    {
        BuffTable = buffTable;
        _target = target;
        target.AddBuff(this);
    }
    
    public virtual void OnUpdate()
    {
        
    }
    
    public bool IsExpired()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime < BuffTable.Duration)
        {
            return false;
        }

        DeActivate();
        return true;
    }

    public void HandleOverlap()
    {
        switch (BuffTable.BuffOverlapType)
        {
        case eBuffOverlap.Ignore:
            {
                
            }
            break;

        case eBuffOverlap.Reset:
            {
                Reset();
            }
            break;

        case eBuffOverlap.StackNoRefresh:
            {
                AddStack();
            }
            break;

        case eBuffOverlap.StackAndRefresh:
            {
                AddStack();
                _elapsedTime = 0;
            }
            break;

        case eBuffOverlap.RefreshOnly:
            {
                _elapsedTime = 0;
            }
            break;
        }
    }
    
    private void Reset()
    {
        _elapsedTime = 0;
        _stackCount = 1;
    }

    private void AddStack()
    {
        _stackCount = Mathf.Min(++_stackCount, BuffTable.MaxStackCount);
    }
    
    protected int CalcValue()
    {
        if (_value > 0)
        {
            return _value;
        }
        
        if (BuffTable.BuffStackType == eBuffStack.Additive)
        {
            _value = BuffTable.Value * _stackCount;
            return _value;
        }

        var value = 1f + BuffTable.Value.ToRatio();
        var pow = Mathf.Pow(value, _stackCount);
        _value = (int)(pow - 1f) * GameRuleConst.PERCENTAGE_BASE;;
        return _value;
    }
}
using UnityEngine;

public abstract class Buff
{
    public BuffTable.Row BuffTable { get; private set; }
    protected IDefender _target;
    private int _value;
    private float _duration;
    private float _elapsedTime;
    private int _stackCount = 1;
    
    protected virtual void DeActivate()
    {
        BuffTable = null;
        _target.RemoveBuffFlags(BuffTable.BuffEffectType);
        _target = null;
        _value = 0;
        _duration = 0f;
        _elapsedTime = 0f;
        _stackCount = 1;
    }
    
    public virtual void Apply(IDefender target, BuffTable.Row buffTable, float duration)
    {
        _target = target;
        BuffTable = buffTable;
        _duration = duration;
        target.AddBuff(this);
    }
    
    public virtual void OnUpdate()
    {
        if (IsExpired())
        {
            DeActivate();
        }
    }
    
    public bool IsExpired()
    {
        _elapsedTime += Time.deltaTime;
        return _elapsedTime >= _duration;
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
                _elapsedTime = 0f;
                _stackCount = 1;
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
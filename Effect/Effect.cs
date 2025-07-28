using UnityEngine;

internal abstract class Effect
{
    protected EffectTable.Row _effectTable;
    protected float _elapsedTime;
    
    protected virtual void DeActivate()
    {
        _effectTable = null;
        _elapsedTime = 0f;
    }

    public void Initialize(EffectTable.Row effectTable)
    {
        _effectTable = effectTable;
    }

    public virtual void OnUpdate()
    {
        if (IsExpired())
        {
            DeActivate();
        }
    }

    public void Apply(IAttacker owner, IDefender target)
    {
        if (target == null)
        {
            SelfApply(owner);
        }
        else
        {
            ApplyToTarget(owner, target);
        }
    }
    
    private bool IsExpired()
    {
        _elapsedTime += Time.deltaTime;
        return _elapsedTime >= _effectTable.Duration;
    }

    protected virtual void SelfApply(IAttacker owner) { }
    protected abstract void ApplyToTarget(IAttacker owner, IDefender target);
}
using UnityEngine;

public sealed class UnitDamageUI
{
    private readonly ObjectPool<UIDamageText> _damageTextPool = new();
    
    public void Initialize(Transform anchorNode)
    {
        _damageTextPool.Initialize(root: anchorNode);
    }

    public void OnUpdate()
    {
        _damageTextPool.OnUpdate();
    }

    public void SetDamage(long damage)
    {
        var damageText = _damageTextPool.Get();
        damageText.Set(damage);
    }
}
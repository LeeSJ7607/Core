using UnityEngine;

public sealed class UnitDamageUI
{
    private readonly ObjectPool<UIDamageText> _damageTextPool = new();
    private Vector3 _anchorNodePos;
    
    public void Initialize(Vector3 anchorNodePos)
    {
        _anchorNodePos = anchorNodePos;
        _damageTextPool.Initialize(3, 5);
    }

    public void OnUpdate()
    {
        _damageTextPool.OnUpdate();
    }

    public void SetDamage(long damage)
    {
        var damageText = _damageTextPool.Get();
        damageText.Set(damage);
        damageText.transform.position = _anchorNodePos;
    }
}
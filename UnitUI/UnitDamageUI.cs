using UnityEngine;

public sealed class UnitDamageUI
{
    private readonly ObjectPool<UIDamageText> _damageTextPool = new();
    private readonly Vector3 _anchorNodePos;
    
    public UnitDamageUI(Vector3 anchorNodePos)
    {
        _anchorNodePos = anchorNodePos;
    }

    public void Initialize()
    {
        _damageTextPool.Initialize(3, 5);
    }

    public void SetDamage(int damage)
    {
        var damageText = _damageTextPool.Get();
        damageText.Set(damage);
        damageText.transform.position = _anchorNodePos;
    }
}
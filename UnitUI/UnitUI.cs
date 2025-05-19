using UnityEngine;

public sealed class UnitUI
{
    private readonly UnitHealthUI _unitHealthUI = new();
    private readonly UnitDamageUI _unitDamageUI = new();

    public void Initialize(IReadOnlyUnit owner)
    {
        var anchorNode = owner.Tm.GetComponentInChildren<AnchorNode>(true);
        _unitHealthUI.Initialize(anchorNode[eAnchorNode.HP]);
        _unitDamageUI.Initialize(anchorNode[eAnchorNode.Damage]);
    }

    public void OnUpdate()
    {
        _unitDamageUI.OnUpdate();
    }

    public void SetHPAndDamage(Stat stat, long damage)
    {
        _unitHealthUI.SetHP(stat[eStat.HP], stat[eStat.Max_HP]);
        _unitDamageUI.SetDamage(damage);
    }
}
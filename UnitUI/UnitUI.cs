using UnityEngine;

public sealed class UnitUI
{
    private readonly UnitHealthUI _unitHealthUI = new();
    private readonly UnitDamageUI _unitDamageUI = new();

    public void Initialize(IReadOnlyUnit owner)
    {
        var anchorNode = owner.Tm.GetComponentInChildren<AnchorNode>(true);
        _unitHealthUI.Initialize(anchorNode[EAnchorNode.HP]);
        _unitDamageUI.Initialize(anchorNode[EAnchorNode.Damage]);
    }

    public void OnUpdate()
    {
        _unitDamageUI.OnUpdate();
    }

    public void SetHPAndDamage(Stat stat, long damage)
    {
        _unitHealthUI.SetHP(stat[EStat.HP], stat[EStat.Max_HP]);
        _unitDamageUI.SetDamage(damage);
    }
}
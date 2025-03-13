using UnityEngine;

public sealed class UnitUI
{
    private readonly UnitHealthUI _unitHealthUI;
    private readonly UnitDamageUI _unitDamageUI;
    private readonly Camera _mainCam;

    public UnitUI(Unit owner)
    {
        var anchorNode = owner.GetComponent<AnchorNode>();
        _unitHealthUI = new UnitHealthUI(anchorNode[EAnchorNode.HP]);
        _unitDamageUI = new UnitDamageUI(anchorNode[EAnchorNode.Hit]);
        _mainCam = Camera.main ?? throw new System.NullReferenceException("Main camera is missing.");
    }

    public void Initialize()
    {
        _unitHealthUI.Initialize();
        _unitDamageUI.Initialize();
    }

    public void OnUpdate()
    {
        _unitHealthUI.OnUpdate();
    }

    public void SetHPAndDamage(Stat stat, int damage)
    {
        _unitHealthUI.SetHP(stat[EStat.HP], stat[EStat.Max_HP]);
        _unitDamageUI.SetDamage(damage);
    }
    
    public static void Billboard()
    {
        
    }
}
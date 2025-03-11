using UnityEngine;
using UnityEngine.UI;

public sealed class UnitHealthUI
{
    private Slider _sldHP;
    private readonly Vector3 _anchorNodePos;
    
    public UnitHealthUI(Vector3 anchorNodePos)
    {
        _anchorNodePos = anchorNodePos;
    }
    
    public void Initialize()
    {
        CreateHP();
    }

    private void CreateHP()
    {
        var res = AddressableManager.Instance.Get<GameObject>(nameof(UnitHealthUI));
        if (res == null)
        {
            return;
        }

        var obj = Object.Instantiate(res, UIManager.Instance.transform);
        if (!obj.TryGetComponent<Slider>(out var sldHP))
        {
            Debug.LogError("Failed to get a component of the type Slider.");
            return;
        }
        
        _sldHP = sldHP;
    }
    
    public void OnUpdate()
    {
        UpdateHPPos();
    }

    private void UpdateHPPos()
    {
        _sldHP.transform.position = _anchorNodePos;
        UnitUI.Billboard();
    }

    public void SetHP(int curHP, int maxHP)
    {
        _sldHP.value = (float)curHP / maxHP;
    }
}
using UnityEngine;
using UnityEngine.UI;

public sealed class UnitHealthUI
{
    private Slider _sldHP;
    private Vector3 _anchorNodePos;
    
    public void Initialize(Vector3 anchorNodePos)
    {
        _anchorNodePos = anchorNodePos;
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
        if (_sldHP == null)
        {
            return;
        }
        
        _sldHP.transform.position = _anchorNodePos;
        UnitUI.Billboard();
    }

    public void SetHP(long curHP, long maxHP)
    {
        _sldHP.value = (float)curHP / maxHP;
    }
}
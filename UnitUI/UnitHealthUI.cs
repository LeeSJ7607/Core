using UnityEngine;
using UnityEngine.UI;

public sealed class UnitHealthUI
{
    private Image _hpBar;
    
    public void Initialize(Transform anchorNode)
    {
        CreateHP(anchorNode);
    }

    private void CreateHP(Transform anchorNode)
    {
        var res = AddressableManager.Instance.Get<GameObject>(nameof(UnitHealthUI));
        if (res == null)
        {
            return;
        }

        var obj = Object.Instantiate(res, anchorNode);
        if (!obj.TryGetComponent<Image>(out var hpBar))
        {
            Debug.LogError("Failed to get a component of the type Slider.");
            return;
        }
        
        _hpBar = hpBar;
    }
    
    public void SetHP(long curHP, long maxHP)
    {
        _hpBar.fillAmount = (float)curHP / maxHP;
    }
}
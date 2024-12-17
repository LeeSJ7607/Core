using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

internal sealed class ShopView : IMVCView
{
    private Button _btn;

    private void Awake()
    {
        _btn.AddClick(OnClick);
    }
    
    public void SetId(int id)
    {
        Debug.Log(id.ToString());
    }

    private void OnClick()
    {
        //TODO: ShopModel 데이터 변경 가능??
    }
}
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

internal sealed class ShopView : UIBase, IMVCView
{
    private Button _btn;
    private ShopModel _model;

    private void Awake()
    {
        _btn.AddClick(OnClick);
    }

    void IMVCView.Bind(IMVCModel model)
    {
        _model = model as ShopModel;
        _model.ChangeId();
    }
    
    public void SetId(int id)
    {
        Debug.Log(id.ToString());
    }

    private void OnClick()
    {
        _model.ChangeId();
    }
}
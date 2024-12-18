using System;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

internal sealed class ShopView : UIBase, IMVCView
{
    private Button _btn;
    private ShopModel _model;
    private IDisposable _disposable;
    private ShopController _shopController;
    
    public override void Hide()
    {
        _disposable.Dispose();
        base.Hide();
    }

    private void Awake()
    {
        _btn.AddClick(OnClick);
    }

    void IMVCView.Bind(IMVCModel model, IDisposable disposable)
    {
        _model = model as ShopModel;
        _disposable = disposable;

        _shopController = new ShopController(_model, this);
        _shopController.Bind();
    }
    
    public void SetId(int id)
    {
        Debug.Log(id.ToString());
    }

    public void SetCount(int count)
    {
        Debug.Log(count.ToString());
    }

    private async void OnClick()
    {
        _model.ChangeId();
        _model.ChangeCount();
        _model.Execute.OnNext(true);

        var req = new LoginReq();
        req.NickName = "adsf";
        var res = await NetworkManager.Instance.SendAsync<LoginRes>(req);
    }
}
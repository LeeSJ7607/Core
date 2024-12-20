using UnityEngine;

internal abstract class UIPopup : UIBase, IMVCView
{
    public virtual bool CanBackKey { get; } = true;
    
    [SerializeField] private UIButton _btnClose;
    private IMVCController _mvcController;

    protected override void OnDestroy()
    {
        _mvcController?.Release();
        _btnClose.RemoveClick();
        base.OnDestroy();
    }

    protected override void Awake()
    {
        base.Awake();
        _btnClose.AddClick(OnClick_Close);
    }

    protected void CreateMVCController(IMVCController mvcController)
    {
        _mvcController = mvcController;
        _mvcController.Initialize(this);
    }

    protected virtual void OnClick_Close()
    {
        Hide();
    }
}
using UnityEngine;

internal abstract class UIPopup : UIBase
{
    public virtual bool CanBackKey { get; } = true;
    [SerializeField] private ButtonEx _btnClose;

    protected override void OnDestroy()
    {
        _btnClose.RemoveClick();
        base.OnDestroy();
    }

    protected override void Awake()
    {
        base.Awake();
        _btnClose.AddClick(OnClick_Close);
    }

    protected virtual void OnClick_Close()
    {
        Hide();
    }
}
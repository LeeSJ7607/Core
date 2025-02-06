using UnityEngine;

internal abstract class UIPopup : UIBase
{
    public virtual bool CanBackButton => true;
    [SerializeField] private ButtonEx _btnClose;

    protected override void OnDestroy()
    {
        _btnClose.RemoveAllListeners();
        base.OnDestroy();
    }

    protected override void Awake()
    {
        base.Awake();
        _btnClose.AddListener(OnClick_Close);
    }

    protected virtual void OnClick_Close()
    {
        Hide();
    }
}
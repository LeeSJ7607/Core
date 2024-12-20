internal abstract class UIPopup : UIBase, IMVCView
{
    public virtual bool CanBackKey { get; } = true;
}
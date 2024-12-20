internal abstract class UIPopup : UIBase, IMVCView
{
    public virtual bool CanBackKey { get; } = true;
    private IMVCController _mvcController;

    protected override void OnDestroy()
    {
        _mvcController?.Release();
        base.OnDestroy();
    }
    
    protected void CreateMVCController(IMVCController mvcController)
    {
        _mvcController = mvcController;
        _mvcController.Initialize(this);
    }
}
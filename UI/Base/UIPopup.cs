internal class UIPopup<TController> : UIBase, IMVCView 
    where TController : IMVCController, new()
{
    protected TController _mvcController;

    protected override void OnDestroy()
    {
        _mvcController.Release();
        base.OnDestroy();
    }

    protected override void Awake()
    {
        base.Awake();
        _mvcController = new TController();
        _mvcController.Initialize(this);
    }
}
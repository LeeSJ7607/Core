using R3;

internal sealed class ShopController : MVCController
{
    private ShopModel _shopModel;
    private ShopView _shopView;
    
    public ShopController(IMVCModel model, IMVCView view) : base(model, view)
    {
        _shopModel = model as ShopModel;
        _shopView = view as ShopView;
    }

    public void Set()
    {
        _shopModel.Id
                  .Subscribe(_ => _shopView.SetId(_))
                  .AddTo(_disposable);
    }
}
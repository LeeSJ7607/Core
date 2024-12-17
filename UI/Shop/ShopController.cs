using R3;

internal sealed class ShopController : MVCController<ShopModel, ShopView>
{
    public ShopController(ShopModel model, ShopView view) : base(model, view)
    {
        
    }
    
    public void Set()
    {
        _model.Id
              .Subscribe(_ => _view.SetId(_))
              .AddTo(_disposable);
    }
}
using R3;

internal abstract class MVCController<TModel, TView>
    where TModel : IMVCModel, new()
    where TView : IMVCView
{
    protected TModel _model;
    protected TView _view;
    protected readonly CompositeDisposable _disposable = new();
    
    public void Release()
    {
        _model.Dispose();
        _disposable.Dispose();
    }

    public void Initialize(IMVCView view)
    {
        _model = new TModel();
        _view = (TView)view;
        
        OnInitialize();
    }
    
    protected abstract void OnInitialize();
}
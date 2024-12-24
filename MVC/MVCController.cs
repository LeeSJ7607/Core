using R3;

public interface IMVCController
{
    void Release();
    void Initialize(IMVCView view);
}

internal abstract class MVCController<TModel, TView> : IMVCController
    where TModel : IMVCModel, new()
    where TView : IMVCView
{
    protected TModel _model;
    protected TView _view;
    protected readonly CompositeDisposable _disposable = new();
    
    void IMVCController.Release()
    {
        _model.Dispose();
        _disposable.Dispose();
    }

    protected abstract void OnInitialize();
    void IMVCController.Initialize(IMVCView view)
    {
        _model = new TModel();
        _view = (TView)view;
        
        OnInitialize();
    }
}
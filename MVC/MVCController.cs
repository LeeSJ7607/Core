using System;
using R3;

internal abstract class MVCController<TModel, TView> : IDisposable
    where TModel : IMVCModel
    where TView : IMVCView
{
    protected TModel _model;
    protected TView _view;
    protected readonly CompositeDisposable _disposable = new();
    
    protected MVCController(TModel model, TView view)
    {
        _model = model;
        _view = view;
    }
    
    void IDisposable.Dispose()
    {
        _model.Dispose();
        _disposable.Dispose();
    }
}
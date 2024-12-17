using System;
using R3;

internal abstract class MVCController : IDisposable
{
    protected readonly IMVCModel _model;
    protected readonly IMVCView _view;
    protected readonly CompositeDisposable _disposable = new();
    
    protected MVCController(IMVCModel model, IMVCView view)
    {
        _model = model;
        _view = view;
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }
}
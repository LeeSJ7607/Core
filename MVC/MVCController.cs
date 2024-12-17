using System;
using R3;

internal abstract class MVCController : IDisposable
{
    protected readonly CompositeDisposable _disposable = new();
    
    protected MVCController(IMVCModel model, IMVCView view)
    {
        
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }
}
using System;

internal interface IMVCView
{
    void Bind(IMVCModel model, IDisposable disposable);
}
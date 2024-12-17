using System;
using R3;

internal sealed class ShopModel : IMVCModel
{
    public ReadOnlyReactiveProperty<int> Id => _id;
    private readonly ReactiveProperty<int> _id = new();
    
    //TODO: 어디서?
    void IDisposable.Dispose()
    {
        _id.Dispose();
    }

    public void ChangeId()
    {
        _id.Value++;
    }
}
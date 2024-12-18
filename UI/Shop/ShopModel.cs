using System;
using R3;

internal sealed class ShopModel : IMVCModel
{
    public ReadOnlyReactiveProperty<int> Id => _id;
    private readonly ReactiveProperty<int> _id = new();

    public ReadOnlyReactiveProperty<int> Count => _count;
    private readonly ReactiveProperty<int> _count = new();

    public ISubject<bool> Execute => _execute;
    private readonly Subject<bool> _execute = new();
    
    void IDisposable.Dispose()
    {
        _id.Dispose();
        _count.Dispose();
        _execute.Dispose();
    }

    public void ChangeId()
    {
        _id.Value++;
    }

    public void ChangeCount()
    {
        _count.Value++;
    }
}
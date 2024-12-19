using System;
using R3;

internal sealed class SampleModel : IMVCModel
{
    public ReadOnlyReactiveProperty<int> OnRankingPoint => _onRankingPoint;
    private readonly ReactiveProperty<int> _onRankingPoint = new();
    
    void IDisposable.Dispose()
    {
        _onRankingPoint.Dispose();
    }

    public void SetRankingPoint(int point)
    {
        _onRankingPoint.Value = point;
    }
}
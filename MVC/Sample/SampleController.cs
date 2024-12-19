using R3;

internal sealed class SampleController : MVCController<SampleModel, SampleView>
{
    protected override void Initialize()
    {
        _model.OnRankingPoint
              .Subscribe(_ => _view.SetPoint(_))
              .AddTo(_disposable);
    }

    public void SetRankingPoint(int point)
    {
        _model.SetRankingPoint(point);
    }
}
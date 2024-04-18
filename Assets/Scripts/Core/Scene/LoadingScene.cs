internal sealed class LoadingScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        
        _baseCanvas = new LoadingCanvas(transform);
        _baseCanvas.Initialize();
    }
}
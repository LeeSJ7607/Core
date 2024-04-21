using Cysharp.Threading.Tasks;

internal sealed class LoadingScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        _baseCanvas = new LoadingCanvas(transform);
    }

    protected override async UniTask Start()
    {
        await base.Start(); 
        _baseCanvas.Initialize();
    }
}
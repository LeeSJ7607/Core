using Cysharp.Threading.Tasks;

internal sealed class OutGameScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        _baseCanvas = new OutGameCanvas(transform);
    }

    protected override async UniTask Start()
    {
        await base.Start();
        _baseCanvas.Initialize();
    }
}
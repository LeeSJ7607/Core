using Cysharp.Threading.Tasks;

internal sealed class InGameScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        _baseCanvas = new InGameCanvas(transform);
    }

    protected override async UniTask Start()
    {
        await base.Start(); 
        _baseCanvas.Initialize();
    }
}
using Cysharp.Threading.Tasks;

internal sealed class LoginScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        _baseCanvas = new LoginCanvas(transform);
    }

    protected override async UniTask Start()
    {
        await base.Start();
        await AddressableManager.Instance.LoadAssetsAsync(nameof(LoginScene));
        
        _baseCanvas.Initialize();
    }
}
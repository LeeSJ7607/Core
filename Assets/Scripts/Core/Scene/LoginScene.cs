using Cysharp.Threading.Tasks;

internal sealed class LoginScene : BaseScene
{
    protected override async UniTask Start()
    {
        await base.Start();
        ModelManager.Instance.Initialize();
    }
}
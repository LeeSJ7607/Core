using Cysharp.Threading.Tasks;

internal sealed class LoginScene : BaseScene
{
    protected override async UniTask Start()
    {
        ModelManager.Instance.Initialize();
        await UniTask.CompletedTask;
    }
}
using Cysharp.Threading.Tasks;

internal sealed class DevInGameScene
{
    public async UniTask Initialize()
    {
        await LoadingScene.ReleaseAll();
        await AddressableManager.Instance.LoadAssetsAsync(EScene.InGame.ToString());
    }
}
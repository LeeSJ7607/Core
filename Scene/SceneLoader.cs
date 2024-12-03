using UnityEngine.AddressableAssets;

public enum EScene
{
    Login,
    OutGame,
    InGame,
    Loading,
}

internal sealed class SceneLoader : Singleton<SceneLoader>
{
    public EScene CurSceneType = EScene.Login;
    
    public void LoadScene(EScene sceneType)
    {
        CurSceneType = sceneType;
        
        var handle = Addressables.LoadSceneAsync(EScene.Loading.ToString());
        Addressables.Release(handle);
    }
    
    public void LoadNextScene()
    {
        var handle = Addressables.LoadSceneAsync(CurSceneType);
        Addressables.Release(handle);
    }
}
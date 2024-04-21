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
        Addressables.LoadSceneAsync(EScene.Loading.ToString());
    }
    
    public void LoadNextScene()
    {
        Addressables.LoadSceneAsync(CurSceneType);
    }
}
using UnityEngine.SceneManagement;

public enum EScene
{
    Login,
    OutGame,
    InGame,
    Loading,
    End,
}

internal sealed class SceneLoader
{
    public EScene CurSceneType = EScene.Login;
    
    public void LoadScene(EScene sceneType)
    {
        CurSceneType = sceneType;
        SceneManager.LoadSceneAsync(EScene.Loading.ToString());
    }
    
    public void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(CurSceneType.ToString());
    }
}
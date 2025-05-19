using UnityEngine.SceneManagement;

public enum eScene
{
    Login,
    OutGame,
    InGame,
    Loading,
    End,
}

internal sealed class SceneLoader
{
    public eScene CurSceneType = eScene.Login;
    
    public void LoadScene(eScene sceneType)
    {
        CurSceneType = sceneType;
        SceneManager.LoadSceneAsync(eScene.Loading.ToString());
    }
    
    public void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(CurSceneType.ToString());
    }
}
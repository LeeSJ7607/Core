using UnityEditor;
using UnityEditor.SceneManagement;

internal sealed class MenuScene
{
    [MenuItem("Custom/Menu/Scene/Login", false, 0)]
    private static void OpenLoginScene()
    {
        if (!TryFindScenePath(EScene.Login.ToString(), out var scenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(scenePath);
    }
    
    [MenuItem("Custom/Menu/Scene/OutGame", false, 1)]
    private static void OpenOutGameScene()
    {
        if (!TryFindScenePath(EScene.OutGame.ToString(), out var scenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(scenePath);
    }
    
    [MenuItem("Custom/Menu/Scene/InGame", false, 2)]
    private static void OpenInGameScene()
    {
        if (!TryFindScenePath(EScene.InGame.ToString(), out var scenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(scenePath);
    }
    
    [MenuItem("Custom/Menu/Scene/Loading", false, 3)]
    private static void OpenLoadingScene()
    {
        if (!TryFindScenePath(EScene.Loading.ToString(), out var scenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(scenePath);
    }

    private static bool TryFindScenePath(string sceneName, out string scenePath)
    {
        scenePath = null;
        var guids = AssetDatabase.FindAssets($"{sceneName} t:Scene");

        if (guids.IsNullOrEmpty())
        {
            EditorUtility.DisplayDialog("Scene Not Found", $"Scene named '{sceneName}' not found.", "OK");
            return false;
        }

        scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        return true;
    }
}
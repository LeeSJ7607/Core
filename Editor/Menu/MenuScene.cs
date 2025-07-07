using UnityEditor;
using UnityEditor.SceneManagement;

internal sealed class MenuScene
{
    [MenuItem("Joel/Menu/Scene/Login", false, 0)]
    private static void OpenLoginScene()
    {
        if (!TryFindScenePath(eScene.Login.ToString(), out var scenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(scenePath);
    }
    
    [MenuItem("Joel/Menu/Scene/OutGame", false, 1)]
    private static void OpenOutGameScene()
    {
        if (!TryFindScenePath(eScene.OutGame.ToString(), out var scenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(scenePath);
    }
    
    [MenuItem("Joel/Menu/Scene/InGame", false, 2)]
    private static void OpenInGameScene()
    {
        if (!TryFindScenePath(eScene.InGame.ToString(), out var scenePath))
        {
            return;
        }

        EditorSceneManager.OpenScene(scenePath);
    }
    
    [MenuItem("Joel/Menu/Scene/Loading", false, 3)]
    private static void OpenLoadingScene()
    {
        if (!TryFindScenePath(eScene.Loading.ToString(), out var scenePath))
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
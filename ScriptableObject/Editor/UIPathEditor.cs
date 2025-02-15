using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIPathSO))]
internal sealed class UIPathEditor : Editor
{
    private static readonly string KEY_SELECTED_FILE_PATH = $"{typeof(UIPathEditor)}_KEY_SELECTED_FILE_PATH";
    private string _selectedFolderPath;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var uiPathSo = (UIPathSO)target;
        if (uiPathSo == null)
        {
            return;
        }
        
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            _selectedFolderPath = PlayerPrefs.GetString(KEY_SELECTED_FILE_PATH);

            if (GUILayout.Button("UI 프리팹 경로 지정"))
            {
                var folderPath = EditorUtility.OpenFolderPanel("UI 프리팹 경로 지정", _selectedFolderPath, "");
                _selectedFolderPath = $"Assets{folderPath.Split(Application.dataPath)[1]}";
                PlayerPrefs.SetString(KEY_SELECTED_FILE_PATH, _selectedFolderPath);
            }
            
            GUILayout.Label(_selectedFolderPath);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            var btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.normal.textColor = new Color(1f, 0.75f, 0.27f);

            if (GUILayout.Button("Clear", btnStyle))
            {
                uiPathSo.Clear();
            }

            if (GUILayout.Button("Refresh", btnStyle))
            {
                Refresh();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void Refresh()
    {
        var uiPathSo = (UIPathSO)target;
        if (uiPathSo == null)
        {
            return;
        }
        
        uiPathSo.Clear();
        
        var guids = AssetDatabase.FindAssets("t:prefab", new [] { _selectedFolderPath });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var uiBase = AssetDatabase.LoadAssetAtPath<UIBase>(path);
            
            if (uiBase == null)
            {
                continue;
            }

            uiPathSo.Add(uiBase.GetType().Name, Path.GetFileNameWithoutExtension(path));
        }
        
        EditorUtility.SetDirty(target);
    }
}
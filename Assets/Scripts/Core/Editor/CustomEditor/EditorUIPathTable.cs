using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIPathTable))]
internal sealed class EditorUIPathTable : Editor
{
    private const string _keySelectedFilePath = "_keySelectedFilePath";
    private string _selectedFolderPath;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var pathTable = (UIPathTable)target;
        if (pathTable == null)
        {
            return;
        }
        
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            _selectedFolderPath = PlayerPrefs.GetString(_keySelectedFilePath);

            if (GUILayout.Button("UI 프리팹 경로 지정"))
            {
                var folderPath = EditorUtility.OpenFolderPanel("UI 프리팹 경로 지정", _selectedFolderPath, "");
                _selectedFolderPath = $"Assets{folderPath.Split(Application.dataPath)[1]}";
                PlayerPrefs.SetString(_keySelectedFilePath, _selectedFolderPath);
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
                pathTable.Clear();
            }

            if (GUILayout.Button("Refresh", btnStyle))
            {
                TryRefresh();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private bool TryRefresh()
    {
        var pathTable = (UIPathTable)target;
        if (pathTable == null)
        {
            return false;
        }
        
        pathTable.Clear();
        
        var guids = AssetDatabase.FindAssets("t:prefab", new [] { _selectedFolderPath });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var uiBase = AssetDatabase.LoadAssetAtPath<UIBase>(path);
            
            if (uiBase == null)
            {
                continue;
            }

            pathTable.Add(uiBase.GetType().Name, new UIPathTable.Table()
            {
                Path = path, 
                Address = GetAddress(guid)
            });
        }
        
        EditorUtility.SetDirty(target);
        return true;
    }
    
    private string GetAddress(string guid)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var entry = settings.FindAssetEntry(guid);

        return entry.address;
    }
}
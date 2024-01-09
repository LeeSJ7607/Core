using UnityEditor;
using UnityEngine;

internal sealed class AssetImporterWindow : EditorWindow
{
    private const string _keySelectedFilePath = "_keySelectedFilePath";
    private const float _drawMenuBtn = 30;
    private readonly AssetImporterGUI _importerGUI = new();
    private string _selectedFolderPath;
    
    [MenuItem("Tool/AssetImporterTool &Q")]
    public static void Open()
    {
        var tool = GetWindow<AssetImporterWindow>();
        var selectedFilePath = tool._selectedFolderPath = PlayerPrefs.GetString(_keySelectedFilePath);
        
        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            tool._importerGUI.Initialize(selectedFilePath);
        }
    }
    
    private void OnGUI()
    {
        DrawCategory();
        DrawDesc();
        DrawMenu();
    }

    private void DrawCategory()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button("Specify the folder path", GUILayout.Width(140)))
        {
            _selectedFolderPath = EditorUtility.OpenFolderPanel("Specify the folder path", _selectedFolderPath, "");
            _importerGUI.Initialize(_selectedFolderPath);
            PlayerPrefs.SetString(_keySelectedFilePath, _selectedFolderPath);
        }
        
        GUILayout.Label($"{_selectedFolderPath} \t\t Texture Count: {_importerGUI.TextureCnt.ToString()}");
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawDesc()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _importerGUI.Draw();
        EditorGUILayout.EndVertical();
    }

    private void DrawMenu()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        DrawDiffBtn();
        DrawConfirmBtn();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDiffBtn()
    {
        if (!GUILayout.Button("변경된 에셋 보기", GUIUtil.ButtonStyle(), GUILayout.Height(_drawMenuBtn)))
        {
            return;
        }
        
        _importerGUI.ShowDiff();
    }

    private void DrawConfirmBtn()
    {
        if (GUILayout.Button("변경된 에셋 적용", GUIUtil.ButtonStyle(), GUILayout.Height(_drawMenuBtn)))
        {
            Save();
        }
    }
    
    private void Save()
    {
        var changed = _importerGUI.TrySave();
        var msg = changed ? "변경된 에셋을 적용했습니다." : "변경된 에셋이 없습니다.";
        EditorUtility.DisplayDialog("알림", msg, "확인");
    }
}
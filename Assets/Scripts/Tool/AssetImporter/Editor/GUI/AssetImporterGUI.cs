using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal sealed class AssetImporterGUI : EditorWindow
{
    private const float _drawMenuBtn = 30;
    private const string _keySelectedFilePath = "_keySelectedFilePath";
    private readonly IAssetImporterGUI[] _assetImporterGuis;
    private string _selectedFolderPath;
    private int _selectedAssetKindIdx;
    
    public AssetImporterGUI()
    {
        _assetImporterGuis = typeof(IAssetImporterGUI).Assembly.GetExportedTypes()
                                                      .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                                                      .Where(_ => typeof(IAssetImporterGUI).IsAssignableFrom(_))
                                                      .Select(_ => (IAssetImporterGUI)Activator.CreateInstance(_))
                                                      .OrderBy(_ => _.Order)
                                                      .ToArray();
    }
    
    [MenuItem("Tool/AssetImporterTool &Q")]
    public static void Open()
    {
        var tool = GetWindow<AssetImporterGUI>();
        var selectedFilePath = tool._selectedFolderPath = PlayerPrefs.GetString(_keySelectedFilePath);

        if (selectedFilePath.IsNullOrEmpty())
        {
            return;
        }

        foreach (var importerGui in tool._assetImporterGuis)
        {
            importerGui.Initialize(selectedFilePath);
        }
    }
    
    private void OnGUI()
    {
        DrawTopMenu();
        DrawAssets();
        DrawBottomMenu();
    }

    private void DrawTopMenu()
    {
        DrawTitle();
        DrawAssetKind();
    }

    private void DrawTitle()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button("Specify the folder path", GUILayout.Width(140)))
        {
            _selectedFolderPath = EditorUtility.OpenFolderPanel("Specify the folder path", _selectedFolderPath, "");
            foreach (var assetImporterGui in _assetImporterGuis)
            {
                assetImporterGui.Initialize(_selectedFolderPath);
            }
            PlayerPrefs.SetString(_keySelectedFilePath, _selectedFolderPath);
        }

        GUILayout.Label(_selectedFolderPath);
        EditorGUILayout.EndHorizontal();   
    }
    
    private void DrawAssetKind()
    {
        EditorGUILayout.BeginHorizontal();
        for (var type = AssetImporterConsts.AssetKind.Texture; type < AssetImporterConsts.AssetKind.End; type++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            var typeIdx = (int)type;
            var toggleName = $"{type.ToString()} ({_assetImporterGuis[typeIdx].TotalCnt})";
            
            if (GUILayout.Toggle(_selectedAssetKindIdx == typeIdx, toggleName))
            {
                if (_selectedAssetKindIdx != typeIdx)
                {
                    _assetImporterGuis[_selectedAssetKindIdx].ScrollPos = Vector2.zero;
                }

                _selectedAssetKindIdx = typeIdx;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawAssets()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _assetImporterGuis[_selectedAssetKindIdx].Draw();
        EditorGUILayout.EndVertical();
    }

    private void DrawBottomMenu()
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
        
        var isOpen = false;
        foreach (var importerGui in _assetImporterGuis)
        {
            if (importerGui.CanDiff())
            {
                isOpen = true;
                break;
            }
        }
        
        if (!isOpen)
        {
            const string msg = "변경된 에셋이 없습니다.\n에셋을 변경 후, 다시 시도해주세요.";
            EditorUtility.DisplayDialog("알림", msg, "확인");
            return;
        }
        
        AssetImporterTool_Diff.Open(_assetImporterGuis);
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
        var changed = _assetImporterGuis[_selectedAssetKindIdx].TrySave();
        var msg = changed ? "변경된 에셋을 적용했습니다." : "변경된 에셋이 없습니다.";
        EditorUtility.DisplayDialog("알림", msg, "확인");
    }
}
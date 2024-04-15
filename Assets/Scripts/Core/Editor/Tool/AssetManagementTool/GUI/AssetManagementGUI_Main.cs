using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal sealed class AssetManagementGUI_Main : EditorWindow
{
    private const float _drawMenuBtn = 30;
    private const string _keySelectedFilePath = "_keySelectedFilePath";
    private readonly IAssetManagementGUI[] _assetManagementGuis;
    private string _selectedFolderPath;
    private int _selectedAssetKindIdx;
    
    public AssetManagementGUI_Main()
    {
        _assetManagementGuis = typeof(IAssetManagementGUI).Assembly.GetExportedTypes()
                                                          .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                                                          .Where(_ => typeof(IAssetManagementGUI).IsAssignableFrom(_))
                                                          .Select(_ => (IAssetManagementGUI)Activator.CreateInstance(_))
                                                          .OrderBy(_ => _.Order)
                                                          .ToArray();
    }
    
    [MenuItem("Tool/AssetManagementTool &Q")]
    public static void Open()
    {
        var tool = GetWindow<AssetManagementGUI_Main>();
        var selectedFilePath = tool._selectedFolderPath = PlayerPrefs.GetString(_keySelectedFilePath);

        if (selectedFilePath.IsNullOrEmpty())
        {
            return;
        }

        foreach (var importerGui in tool._assetManagementGuis)
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
            foreach (var assetManagementGUI in _assetManagementGuis)
            {
                assetManagementGUI.Initialize(_selectedFolderPath);
            }
            PlayerPrefs.SetString(_keySelectedFilePath, _selectedFolderPath);
        }

        GUILayout.Label(_selectedFolderPath);
        EditorGUILayout.EndHorizontal();   
    }
    
    private void DrawAssetKind()
    {
        EditorGUILayout.BeginHorizontal();
        for (var type = AssetManagementConsts.AssetKind.Texture; type < AssetManagementConsts.AssetKind.End; type++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            var typeIdx = (int)type;
            var toggleName = $"{type.ToString()} ({_assetManagementGuis[typeIdx].TotalCnt})";
            
            if (GUILayout.Toggle(_selectedAssetKindIdx == typeIdx, toggleName))
            {
                if (_selectedAssetKindIdx != typeIdx)
                {
                    _assetManagementGuis[_selectedAssetKindIdx].ScrollPos = Vector2.zero;
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
        _assetManagementGuis[_selectedAssetKindIdx].Draw();
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
        foreach (var importerGui in _assetManagementGuis)
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
        
        AssetManagementTool_Diff.Open(_assetManagementGuis);
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
        var changed = false;
        
        foreach (var assetManagementGUI in _assetManagementGuis)
        {
            if (assetManagementGUI.TrySave())
            {
                changed = true;
            }
        }
        
        var msg = changed ? "변경된 에셋을 적용했습니다." : "변경된 에셋이 없습니다.";
        EditorUtility.DisplayDialog("알림", msg, "확인");
    }
}
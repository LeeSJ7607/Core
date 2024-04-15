using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetManagementGUI_FBX : IAssetManagementGUI
{
    private const int _drawMaxRow = 5;
    
    public int Order => 1;
    public int TotalCnt => _fbxImpl.TotalCnt;
    public Vector2 ScrollPos { get; set; }
    public IAssetManagementImpl OriginAssetManagementImpl => _originFBXImpl;
    public IAssetManagementImpl AssetManagementImpl => _fbxImpl;
    private readonly AssetManagementImpl_FBX _originFBXImpl = new();
    private readonly AssetManagementImpl_FBX _fbxImpl = new();
    private int _selectedFBXPathIdx;
    private int _selectedFBXSortIdx;
    private readonly string[] _sortFBX = Enum.GetNames(typeof(AssetManagementConsts.SortFBX)).ToArray();
    private List<string> _fbxDirPaths;
    private List<string> _btnNameFbxDirPaths;
    private Texture2D _texModified;
    
    private void Clear()
    {
        _fbxDirPaths?.Clear();
        _btnNameFbxDirPaths?.Clear();
    }

    public void Initialize(string selectedFilePath)
    {
        Clear();
        AssetManagementGUI_Texture.SetDirPath(selectedFilePath, "t:Model", ref _fbxDirPaths, ref _btnNameFbxDirPaths);
        
        if (_fbxDirPaths != null)
        {
            _originFBXImpl.Initialize(_fbxDirPaths);
            _fbxImpl.Initialize(_fbxDirPaths);
        }
        
        _texModified ??= Resources.Load<Texture2D>("AssetManagementTool_Modified");
    }
    
    private bool IsValid()
    {
        return _btnNameFbxDirPaths is {Count: > 0};
    }

    public void Draw()
    {
        if (!IsValid())
        {
            return;
        }
        
        DrawFolder();
        DrawMenus();
        DrawAssets();
    }

    private void DrawFolder()
    {
        EditorGUILayout.BeginHorizontal();

        for (var i = 0; i < _btnNameFbxDirPaths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            
            var cnt = _fbxImpl.SearchedCnt(_fbxDirPaths[i]);
            var toggleName = $"{_btnNameFbxDirPaths[i]} ({cnt.ToString()})";
            
            if (GUILayout.Toggle(_selectedFBXPathIdx == i, toggleName, GUILayout.ExpandWidth(true)))
            {
                if (_selectedFBXPathIdx != i)
                {
                    ScrollPos = Vector2.zero;
                }

                _selectedFBXPathIdx = i;
                _fbxImpl.CalcSearchedAssetInfos(_fbxDirPaths[_selectedFBXPathIdx]);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawMenus()
    {
        GUIUtil.Btn("모든 참조 찾기", () =>
        {
            DependencyUtil.Dependencies(_fbxImpl.SearchedAssetInfos);
            Sort((int)AssetManagementConsts.SortFBX.References, true);
        });
        
        DrawSort();
    }
    
    private void DrawSort()
    {
        EditorGUILayout.BeginHorizontal();
        GUIUtil.DrawPopup("정렬", ref _selectedFBXSortIdx, _sortFBX, () => Sort(_selectedFBXSortIdx, false));
        GUIUtil.Btn("▲", 25, () => Sort(_selectedFBXSortIdx, false));
        GUIUtil.Btn("▼", 25, () => Sort(_selectedFBXSortIdx, true));
        EditorGUILayout.EndHorizontal();
    }
    
    private void Sort(int sortIdx, bool descending)
    {
        _fbxImpl.CurSort = ((AssetManagementConsts.SortFBX)sortIdx, descending);
        _fbxImpl.CalcSearchedAssetInfos(_fbxDirPaths[_selectedFBXPathIdx]);
    }

    private void DrawAssets()
    {
        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
        
        var totalCnt = _fbxImpl.SearchedAssetInfos.Count;
        for (var i = 0; i < totalCnt; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var j = 0; j < _drawMaxRow; j++)
            {
                var idx = i + j;
                if (idx >= totalCnt)
                {
                    break;
                }
                
                var assetInfo = _fbxImpl.SearchedAssetInfos[idx];
                EditorGUILayout.BeginHorizontal(GUIUtil.HelpBoxStyle(assetInfo.Changed ? _texModified : null), GUILayout.Width(375));
                DrawDesc(assetInfo);
                DrawOption(assetInfo);
                EditorGUILayout.EndHorizontal();
            }
            
            i += (_drawMaxRow - 1);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawDesc(AssetManagementImpl_FBX.AssetInfo assetInfo)
    {
        const float keyWidth = 110;
        const float valueWidth = 220;
        
        EditorGUILayout.Space(1);
        GUILayout.BeginVertical();
        
        GUIUtil.Desc("Name", assetInfo.FBX.name, keyWidth, valueWidth);
        GUIUtil.Desc("Normals", assetInfo.Normals.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Tangents", assetInfo.Tangents.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Mesh Compression", assetInfo.MeshCompression.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Read/Write", assetInfo.IsReadable ? "O" : "X", keyWidth, valueWidth);
        GUIUtil.Desc("File Size", assetInfo.FileSizeStr, keyWidth, valueWidth);
        
        GUILayout.EndVertical();
    }

    private void DrawOption(AssetManagementImpl_FBX.AssetInfo assetInfo)
    {
        const float width = 50;
        
        EditorGUILayout.BeginVertical();
        
        GUIUtil.Btn("선택", width, () => Selection.activeObject = assetInfo.FBX);
        GUIUtil.Btn("열기", width, () => EditorUtility.RevealInFinder(assetInfo.ModelImporter.assetPath));
        GUIUtil.Btn("수정", width, () => AssetManagementTool_FBXModify.Open(assetInfo));
        GUIUtil.Btn("리셋", width, assetInfo.Reset);
        
        if (assetInfo.IsReferences)
        {
            GUIUtil.Btn("참조", () =>
            {
                AssetManagementTool_ReferenceList.Open(new AssetManagementTool_ReferenceList.ReferenceParam(
                    AssetManagementConsts.AssetKind.FBX, assetInfo.References, assetInfo.FileSizeStr));
            });
        }
        
        EditorGUILayout.EndVertical();
    }
    
    public bool CanDiff() => _fbxImpl.CanDiff();
    public bool TrySave() => _fbxImpl.TrySave();
}
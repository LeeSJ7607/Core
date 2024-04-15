using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetManagementGUI_Texture : IAssetManagementGUI
{
    private const int _filterWidth = 952;
    private const int _drawMaxRow = 5;
    
    public int Order => 0;
    public int TotalCnt => _textureImpl.TotalCnt;
    public Vector2 ScrollPos { get; set; }
    public IAssetManagementImpl OriginAssetManagementImpl => _originTextureImpl;
    public IAssetManagementImpl AssetManagementImpl => _textureImpl;
    private readonly AssetManagementImpl_Texture _originTextureImpl = new();
    private readonly AssetManagementImpl_Texture _textureImpl = new();
    private int _selectedTextureFormatIdx = Array.FindIndex(AssetManagementImpl_Texture.TextureFormats, _ => _.Equals(TextureImporterFormat.ASTC_6x6.ToString()));
    private int _selectedTextureFilterIdx;
    private readonly string[] _filterTextures = Enum.GetNames(typeof(AssetManagementConsts.FilterTexture)).ToArray();
    private int _selectedTextureSortIdx;
    private readonly string[] _sortTextures = Enum.GetNames(typeof(AssetManagementConsts.SortTexture)).ToArray();
    private int _selectedTextureMaxSizeIdx;
    private int _selectedTextureMinSizeIdx = AssetManagementImpl_Texture.TextureSizes.Length - 1;
    private int _selectedLabelIdx;
    private string _searchedTextureName;
    private int _selectedTexturePathIdx;
    private int _selectedAssetTypeIdx;
    private List<string> _textureDirPaths;
    private List<string> _btnNameTextureDirPaths;
    private List<string> _fbxDirPaths;
    private List<string> _btnNameFbxDirPaths;
    private Texture2D _texModified;
    
    private void Clear()
    {
        _textureDirPaths?.Clear();
        _btnNameTextureDirPaths?.Clear();
    }

    public void Initialize(string selectedFilePath)
    {
        Clear();
        SetDirPath(selectedFilePath, "t:texture", ref _textureDirPaths, ref _btnNameTextureDirPaths);

        if (_textureDirPaths != null)
        {
            _originTextureImpl.Initialize(_textureDirPaths);
            _textureImpl.Initialize(_textureDirPaths);
        }
        
        _texModified ??= Resources.Load<Texture2D>("AssetManagementTool_Modified");
    }

    public static void SetDirPath(
        string selectedFilePath, 
        string filter, 
        ref List<string> calcDirPaths, 
        ref List<string> btnNamePaths)
    {
        var path = selectedFilePath.Split(Application.dataPath);
        if (path.Length < 2)
        {
            return;
        }

        var fullPath = $"Assets{path[1]}";
        var directoryPaths = Directory.GetDirectories(fullPath).ToList();
        directoryPaths.Add(fullPath);
        calcDirPaths = new List<string>(directoryPaths.Count);

        foreach (var dirPath in directoryPaths)
        {
            var guids = AssetDatabase.FindAssets(filter, new[] { dirPath });
            if (guids == null || guids.Length == 0)
            {
                continue;
            }
            
            calcDirPaths.Add(dirPath);
        }

        btnNamePaths = calcDirPaths.Select(Path.GetFileNameWithoutExtension).ToList();
    }
   
    private bool IsValid()
    {
        return _btnNameTextureDirPaths is {Count: > 0};
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
        for (var i = 0; i < _btnNameTextureDirPaths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            
            var cnt = _textureImpl.SearchedCnt(_textureDirPaths[i]);
            var toggleName = $"{_btnNameTextureDirPaths[i]} ({cnt.ToString()})";
            
            if (GUILayout.Toggle(_selectedTexturePathIdx == i, toggleName, GUILayout.ExpandWidth(true)))
            {
                if (_selectedTexturePathIdx != i)
                {
                    ScrollPos = Vector2.zero;
                }

                _selectedTexturePathIdx = i;
                CalcSearchedAssetInfos();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawMenus()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.Btn("모든 참조 찾기", () =>
            {
                DependencyUtil.Dependencies(_textureImpl.SearchedAssetInfos);
                Sort((int)AssetManagementConsts.SortTexture.References, true);
            });
            GUIUtil.Btn("동일한 텍스쳐 모두 찾기", () =>
            {
                DependencyUtil.SameAssets(_textureImpl.SearchedAssetInfos);
                Sort((int)AssetManagementConsts.SortTexture.Compare, true);
            });
        }
        EditorGUILayout.EndHorizontal();
        
        DrawTextureFormat();
        DrawSortAndFilter();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.DrawPopup("텍스쳐 최대 사이즈", ref _selectedTextureMaxSizeIdx, AssetManagementImpl_Texture.TextureSizes, CalcSearchedAssetInfos);
            GUIUtil.DrawPopup("텍스쳐 최소 사이즈", ref _selectedTextureMinSizeIdx, AssetManagementImpl_Texture.TextureSizes, CalcSearchedAssetInfos);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            GUILayout.Label("텍스쳐 이름 검색", GUILayout.Width(150));
            _searchedTextureName = GUILayout.TextField(_searchedTextureName, GUIUtil.TextFieldStyle(), GUILayout.Width(690));
            GUIUtil.Btn("파일 이름 변경", 100, () => AssetManagementTool_ChangeName.Open(_textureImpl.SearchedAssetInfos));
            GUIUtil.DrawPopup("레이블 검색", ref _selectedLabelIdx, _textureImpl.Labels, CalcSearchedAssetInfos);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void CalcSearchedAssetInfos()
    {
        _textureImpl.CalcSearchedAssetInfos(_textureDirPaths[_selectedTexturePathIdx], _selectedLabelIdx, _selectedTextureMaxSizeIdx, _selectedTextureMinSizeIdx, _searchedTextureName);
    }
    
    private void DrawTextureFormat()
    {
        EditorGUILayout.BeginHorizontal();
        GUIUtil.DrawPopup("텍스쳐 압축 포맷", ref _selectedTextureFormatIdx, AssetManagementImpl_Texture.TextureFormats, _filterWidth);
        GUIUtil.Btn("전체 텍스쳐 압축 포맷 지정", () => Set(true));
        GUIUtil.Btn("전체 텍스쳐 압축 포맷 취소", () => Set(false));
        EditorGUILayout.EndHorizontal();

        void Set(bool active)
        {
            foreach (var assetInfo in _textureImpl.SearchedAssetInfos)
            {
                if (active)
                {
                    assetInfo.SetTextureImporterFormat(_selectedTextureFormatIdx, false);
                }
                else
                {
                    assetInfo.ReSetTextureImporterFormat();
                }
            }
        }
    }
    
    private void DrawSortAndFilter()
    {
        EditorGUILayout.BeginHorizontal();
        GUIUtil.DrawPopup("필터", ref _selectedTextureFilterIdx, _filterTextures, _filterWidth, () => Filter(_selectedTextureFilterIdx));
        GUIUtil.DrawPopup("정렬", ref _selectedTextureSortIdx, _sortTextures, () => Sort(_selectedTextureSortIdx, false));
        GUIUtil.Btn("▲", 25, () => Sort(_selectedTextureSortIdx, false));
        GUIUtil.Btn("▼", 25, () => Sort(_selectedTextureSortIdx, true));
        EditorGUILayout.EndHorizontal();
    }
    
    private void Sort(int sortIdx, bool descending)
    {
        _textureImpl.CurSort = ((AssetManagementConsts.SortTexture)sortIdx, descending);
        CalcSearchedAssetInfos();
    }
    
    private void Filter(int idx)
    {
        _textureImpl.CurFilterType = (AssetManagementConsts.FilterTexture)idx;
        CalcSearchedAssetInfos();
    }
    
    private void DrawAssets()
    {
        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);

        var totalCnt = _textureImpl.SearchedAssetInfos.Count;
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

                var assetInfo = _textureImpl.SearchedAssetInfos[idx];
                EditorGUILayout.BeginHorizontal(GUIUtil.HelpBoxStyle(assetInfo.Changed ? _texModified : null), GUILayout.Width(375));
                DrawTexture(assetInfo);
                DrawDesc(assetInfo);
                DrawOption(assetInfo);
                EditorGUILayout.EndHorizontal();
            }

            i += (_drawMaxRow - 1);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawTexture(AssetManagementImpl_Texture.AssetInfo assetInfo)
    {
        var tex = assetInfo.Texture2D;
        GUIUtil.Btn(tex, 50, 50, () => AssetManagementTool_Preview.Open(tex));
    }
    
    private void DrawDesc(AssetManagementImpl_Texture.AssetInfo assetInfo)
    {
        const float keyWidth = 80;
        const float valueWidth = 180;
        var tex = assetInfo.Texture2D;
        
        EditorGUILayout.Space(1);
        GUILayout.BeginVertical();
        GUIUtil.Desc("Name", tex.name, keyWidth, valueWidth);
        GUIUtil.Desc("Texture Type", assetInfo.TextureType.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Wrap Mode", assetInfo.WrapMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Filter Mode", assetInfo.FilterMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Max Size", assetInfo.MaxTextureSize.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Format", assetInfo.FormatType.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Texture Size", $"{tex.width.ToString()}x{tex.height.ToString()}", keyWidth, valueWidth);
        GUIUtil.Desc("File Size", assetInfo.FileSizeStr, keyWidth, valueWidth);
        GUIUtil.Desc("MipMap", assetInfo.TextureImporter.mipmapEnabled ? "O" : "X", keyWidth, valueWidth);
        GUILayout.EndVertical();
    }
    
    private void DrawOption(AssetManagementImpl_Texture.AssetInfo assetInfo)
    {
        const float width = 50;

        EditorGUILayout.BeginVertical();
        
        GUIUtil.Btn("선택", width, () => Selection.activeObject = assetInfo.Texture2D);
        GUIUtil.Btn("열기", width, () => EditorUtility.RevealInFinder(assetInfo.TextureImporter.assetPath));
        GUIUtil.Btn("포맷", width, () => assetInfo.SetTextureImporterFormat(_selectedTextureFormatIdx, true));
        GUIUtil.Btn("수정", width, () => AssetManagementTool_TextureModify.Open(assetInfo));
        GUIUtil.Btn("리셋", width, assetInfo.Reset);

        if (assetInfo.IsReferences)
        {
            GUIUtil.Btn("참조", width, () =>
            {
                AssetManagementTool_ReferenceList.Open(new AssetManagementTool_ReferenceList.ReferenceParam(
                    AssetManagementConsts.AssetKind.Texture, assetInfo.References, assetInfo.FileSizeStr));
            });
        }
        if (assetInfo.IsCompare)
        {
            GUIUtil.Btn("비교", width, () => AssetManagementTool_CompareList.Open(assetInfo));
        }

        EditorGUILayout.EndVertical();
    }

    public bool CanDiff() => _textureImpl.CanDiff();
    public bool TrySave() => _textureImpl.TrySave();
}
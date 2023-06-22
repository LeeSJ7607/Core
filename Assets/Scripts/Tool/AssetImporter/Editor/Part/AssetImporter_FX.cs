using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporter_FX : AssetImporterPart
{
    private const int _filterWidth = 952;
    private const int _drawMaxRow = 5; //TODO: 외부에서 변경이 가능하도록.

    public override string Name => "FX";

    public override bool IsOn
    {
        get => _isOn;
        set
        {
            _isOn = value;

            if (value)
            {
                Initialize();
            }
        }
    }
    private bool _isOn;
    
    private readonly AssetImporter_TextureImpl _originTextureImpl = new();
    private readonly AssetImporter_TextureImpl _textureImpl = new();
    private int _selectedTextureFormatIdx = Array.FindIndex(AssetImporter_TextureImpl.TextureFormats, _ => _.Equals(TextureImporterFormat.ASTC_6x6.ToString()));
    private int _selectedTextureMaxSizeIdx;
    private int _selectedTextureMinSizeIdx = AssetImporter_TextureImpl.TextureSizes.Length - 1;
    private int _selectedLabelIdx;
    private Texture2D _texModified;
    private Vector2 _scrollPos;
    private string _searchedTextureName;
    private List<string> _texturePaths;
    private string[] _btnNameTexturePaths;
    private int _selectedTexturePathIdx;
    private bool _initialized;
    
    private void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        //TODO: 저장된 DrawMaxRow를 적용한다.
        //_drawMaxRow
        
        //TODO: 저장된 TexModified를 적용한다.
        _texModified = Resources.Load<Texture2D>("AssetImporterTool_TexModified");

        var path = GetTexturePaths();
        _originTextureImpl.Initialize(path);
        _textureImpl.Initialize(path);
    }

    private IEnumerable<string> GetTexturePaths()
    {
        var paths = Directory.GetDirectories("Assets/Temps");
        _texturePaths = new List<string>(paths.Length);

        foreach (var path in paths)
        {
            var guids = AssetDatabase.FindAssets("t:texture", new[] { path });
            if (guids == null || guids.Length == 0)
            {
                continue;
            }
            
            _texturePaths.Add(path);
        }

        _btnNameTexturePaths = _texturePaths.Select(Path.GetFileNameWithoutExtension).ToArray();
        return _texturePaths;
    }
    
    public override void Draw()
    {
        DrawMenus();
        DrawAssets();
    }
    
    private void DrawMenus()
    {
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.Btn("모든 참조 찾기", () =>
            {
                DependencyImpl.Dependencies(_textureImpl.SearchedAssetInfos);
                Sort((int)SortTexture.References, true);
            });
            GUIUtil.Btn("동일한 텍스쳐 모두 찾기", () =>
            {
                DependencyImpl.SameAssets(_textureImpl.SearchedAssetInfos);
                Sort((int)SortTexture.Compare, true);
            });
        }
        EditorGUILayout.EndHorizontal();
        
        DrawTextureFormat();
        DrawSortAndFilter();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.DrawPopup("텍스쳐 최대 사이즈", ref _selectedTextureMaxSizeIdx, AssetImporter_TextureImpl.TextureSizes, CalcSearchedAssetInfos);
            GUIUtil.DrawPopup("텍스쳐 최소 사이즈", ref _selectedTextureMinSizeIdx, AssetImporter_TextureImpl.TextureSizes, CalcSearchedAssetInfos);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            GUILayout.Label("텍스쳐 이름 검색", GUILayout.Width(150));
            _searchedTextureName = GUILayout.TextField(_searchedTextureName, GUIUtil.TextFieldStyle(), GUILayout.Width(690));
            GUIUtil.Btn("파일 이름 변경", 100, () => AssetImporterTool_ChangeName.Open(_textureImpl.SearchedAssetInfos));
            GUIUtil.DrawPopup("레이블 검색", ref _selectedLabelIdx, _textureImpl.Labels, CalcSearchedAssetInfos);
        }
        EditorGUILayout.EndHorizontal();
        
        DrawFolder();
    }

    private void DrawFolder()
    {
        EditorGUILayout.BeginHorizontal();
        for (var i = 0; i < _btnNameTexturePaths.Length; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Toggle(_selectedTexturePathIdx == i, _btnNameTexturePaths[i], GUILayout.ExpandWidth(true)))
            {
                if (_selectedTexturePathIdx != i)
                {
                    _scrollPos = Vector2.zero;
                }

                _selectedTexturePathIdx = i;
                CalcSearchedAssetInfos();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void CalcSearchedAssetInfos()
    {
        _textureImpl.CalcSearchedAssetInfos(_texturePaths[_selectedTexturePathIdx], _selectedLabelIdx, _selectedTextureMaxSizeIdx, _selectedTextureMinSizeIdx, _searchedTextureName);
    }
    
    private void DrawTextureFormat()
    {
        EditorGUILayout.BeginHorizontal();
        GUIUtil.DrawPopup("텍스쳐 압축 포맷", ref _selectedTextureFormatIdx, AssetImporter_TextureImpl.TextureFormats, _filterWidth);
        GUIUtil.Btn("전체 텍스쳐 압축 포맷 지정", () => Set(true));
        GUIUtil.Btn("전체 텍스쳐 압축 포맷 취소", () => Set(false));
        EditorGUILayout.EndHorizontal();

        void Set(bool active)
        {
            foreach (var assetInfo in _textureImpl.SearchedAssetInfos)
            {
                if (active)
                {
                    var formatStr = AssetImporter_TextureImpl.TextureFormats[_selectedTextureFormatIdx];
                    var format = Enum.Parse<TextureImporterFormat>(formatStr);
                    assetInfo.SetTextureImporterFormat(format, true);
                }
                else
                {
                    assetInfo.SetTextureImporterFormat(assetInfo.FormatType, false);
                }
            }
        }
    }
    
    private void DrawSortAndFilter()
    {
        EditorGUILayout.BeginHorizontal();
        GUIUtil.DrawPopup("필터", ref _selectedTextureFilterIdx, _filterTextures, _filterWidth, () => Filter(_selectedTextureFilterIdx));
        
        GUIUtil.DrawPopup("정렬", ref _selectedTextureSortIdx, _sortTextures, () => Sort(_selectedTextureSortIdx, false));
        GUIUtil.Btn("▼", 25, () => Sort(_selectedTextureSortIdx, true));
        GUIUtil.Btn("▲", 25, () => Sort(_selectedTextureSortIdx, false));
        EditorGUILayout.EndHorizontal();
    }
    
    private void Sort(int sortIdx, bool descending)
    {
        _textureImpl.CurSort = ((SortTexture)sortIdx, descending);
        CalcSearchedAssetInfos();
    }
    
    private void Filter(int idx)
    {
        _textureImpl.CurFilterType = (FilterTexture)idx;
        CalcSearchedAssetInfos();
    }
    
    private void DrawAssets()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

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
    
    private void DrawTexture(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        var tex = assetInfo.Texture2D;
        GUIUtil.Btn(tex, 50, 50, () => AssetImporterTool_Preview.Open(tex));
    }
    
    //TODO: 설명 우선 순위도 결정하게 해주면 좋을듯.
    //TODO: 폰트 크기도 수정해주면 좋을듯.
    private void DrawDesc(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        const float keyWidth = 80;
        const float valueWidth = 170;
        var tex = assetInfo.Texture2D;
        
        EditorGUILayout.Space(1);
        GUILayout.BeginVertical();
        GUIUtil.Desc("Name", tex.name, keyWidth, valueWidth);
        GUIUtil.Desc("Texture Type", assetInfo.TextureType.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Wrap Mode", assetInfo.WrapMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Filter Mode", assetInfo.FilterMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Max Size", assetInfo.MaxTextureSize.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Format", assetInfo.FormatStr, keyWidth, valueWidth);
        GUIUtil.Desc("Texture Size", $"{tex.width.ToString()}x{tex.height.ToString()}", keyWidth, valueWidth);
        GUILayout.EndVertical();
    }
    
    private void DrawOption(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        const float width = 50;

        EditorGUILayout.BeginVertical();
        
        GUIUtil.Btn("선택", width, () => Selection.activeObject = assetInfo.Texture2D);
        GUIUtil.Btn("열기", width, () => EditorUtility.RevealInFinder(assetInfo.TextureImporter.assetPath));
        GUIUtil.Btn("수정", width, () => AssetImporterTool_Modify.Open(assetInfo));
        GUIUtil.Btn("리셋", width, assetInfo.Reset);

        if (assetInfo.IsReferences)
        {
            GUIUtil.Btn("참조", width, () => AssetImporterTool_ReferenceList.Open(assetInfo));
        }
        if (assetInfo.IsCompare)
        {
            GUIUtil.Btn("비교", width, () => AssetImporterTool_CompareList.Open(assetInfo));
        }

        EditorGUILayout.EndVertical();
    }
    
    public override void ShowDiff()
    {
        if (_textureImpl.CanDiff() == false)
        {
            const string msg = "변경된 에셋이 없습니다.\n에셋을 변경 후, 다시 시도해주세요.";
            EditorUtility.DisplayDialog("알림", msg, "확인");
            return;
        }
        
        AssetImporterTool_Diff.Open(this, _originTextureImpl, _textureImpl);
    }

    public override bool TrySave() => _textureImpl.TrySave();
}
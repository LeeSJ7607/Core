using System;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporter_FX : AssetImporterPart
{
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
    
    private readonly AssetImporter_TextureImpl _textureImpl = new();
    private AssetImporter_TextureImpl.AssetInfo _compareAssetInfo;
    private int _selectedTextureFormatIdx = Array.FindIndex(AssetImporter_TextureImpl.TextureFormats, _ => _.Equals(TextureImporterFormat.ASTC_6x6.ToString()));
    private int _selectedTextureMaxSizeIdx;
    private int _selectedTextureMinSizeIdx = AssetImporter_TextureImpl.TextureSizes.Length - 1;
    private int _selectedLabelIdx;
    private int _drawMaxRow = 5;
    private Texture2D _texModified;
    private Vector2 _scrollPos;
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

        _textureImpl.Initialize();
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
            if (GUILayout.Button("전체 참조 찾기"))
            {
                AssetImporterUtil.AllAssetCalcReferences(_textureImpl.SearchedAssetInfos);
                AssetImporterTool.ToolMode = ToolMode.References;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            _selectedTextureFormatIdx = EditorGUILayout.Popup("텍스쳐 압축 포맷", _selectedTextureFormatIdx, AssetImporter_TextureImpl.TextureFormats);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.DrawPopup("텍스쳐 최대 사이즈", ref _selectedTextureMaxSizeIdx, AssetImporter_TextureImpl.TextureSizes, CalcSearchedAssetInfos);
            GUIUtil.DrawPopup("텍스쳐 최소 사이즈", ref _selectedTextureMinSizeIdx, AssetImporter_TextureImpl.TextureSizes, CalcSearchedAssetInfos);
        }
        EditorGUILayout.EndHorizontal();
        
        GUIUtil.DrawPopup("레이블 검색", ref _selectedLabelIdx, _textureImpl.Labels, CalcSearchedAssetInfos);

        void CalcSearchedAssetInfos()
        {
            _textureImpl.CalcSearchedAssetInfos(_selectedLabelIdx, _selectedTextureMaxSizeIdx, _selectedTextureMinSizeIdx);
        }
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
                if (CanDrawAsset(assetInfo) == false)
                {
                    continue;
                }

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

    private bool CanDrawAsset(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        switch (AssetImporterTool.ToolMode)
        { 
        case ToolMode.References when assetInfo.IsReferences == false:
        case ToolMode.Compare when assetInfo.IsCompare == false:
        case ToolMode.Compare when _compareAssetInfo.IsSame(assetInfo):
            return false;
        }
        return true;
    }
    
    private void DrawTexture(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        var tex = assetInfo.Texture2D;
        
        if (GUILayout.Button(tex, GUILayout.Width(50), GUILayout.Height(50)))
        {
            AssetImporterTool_Preview.Open(tex);
        }
    }
    
    //TODO: 설명 우선 순위도 결정하게 해주면 좋을듯.
    //TODO: 폰트 크기도 수정해주면 좋을듯.
    private void DrawDesc(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        const float keyWidth = 80;
        const float valueWidth = 170;
        var tex = assetInfo.Texture2D;
        var importer = assetInfo.TextureImporter;
        
        EditorGUILayout.Space(1);
        GUILayout.BeginVertical();
        GUIUtil.Desc("Name", tex.name, keyWidth, valueWidth);
        GUIUtil.Desc("Texture Type", assetInfo.TextureType.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Wrap Mode", assetInfo.WrapMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Filter Mode", assetInfo.FilterMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Max Size", importer.maxTextureSize.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("format", assetInfo.AOSSettings.format.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Texture Size", $"{tex.width.ToString()}x{tex.height.ToString()}", keyWidth, valueWidth);
        GUILayout.EndVertical();
    }
    
    private void DrawOption(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        EditorGUILayout.BeginVertical();
        
        Btn("선택", () => Selection.activeObject = assetInfo.Texture2D);
        Btn("열기", () => EditorUtility.RevealInFinder(assetInfo.TextureImporter.assetPath));
        Btn("수정", () => AssetImporterTool_Modify.Open(assetInfo));
        Btn("포맷", () => assetInfo.SetTextureImporterFormat(_selectedTextureFormatIdx));
        Btn("참조", () => References(assetInfo));
        Btn("비교", () => Compare(assetInfo));

        EditorGUILayout.EndVertical();
        
        void Btn(string name, Action act)
        {
            if (GUILayout.Button(name, GUIUtil.ButtonStyle(), GUILayout.Width(50)))
            {
                act();
            }
        }
    }

    private void References(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        End(ToolMode.Compare);
        
        if (assetInfo.References != null)
        {
            AssetImporterTool_References.Open(assetInfo.References);
            return;
        }
        
        AssetImporterTool_References.Open(assetInfo.Texture2D);
    }

    private void Compare(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        End(ToolMode.References);
        
        if (AssetImporterTool.ToolMode == ToolMode.Compare)
        {
            AssetImporterTool_Compare.Open(_compareAssetInfo, assetInfo);
            return;
        }
        
        AssetImporterTool.ToolMode = ToolMode.Compare;
        
        _compareAssetInfo ??= assetInfo;
        EditorTextureUtil.ChangeReadable(_compareAssetInfo.TextureImporter, true);

        foreach (var searchedAssetInfo in _textureImpl.SearchedAssetInfos)
        {
            EditorTextureUtil.ChangeReadable(searchedAssetInfo.TextureImporter, true);
            if (EditorTextureUtil.IsSameTexture(searchedAssetInfo.Texture2D, _compareAssetInfo.Texture2D))
            {
                searchedAssetInfo.IsCompare = true;
            }
        }
    }

    public override void End(ToolMode toolMode)
    {
        switch (toolMode)
        {
        case ToolMode.Compare:
            {
                foreach (var searchedAssetInfo in _textureImpl.SearchedAssetInfos)
                {
                    EditorTextureUtil.ChangeReadable(searchedAssetInfo.TextureImporter, searchedAssetInfo.isOriginReadable);
                }
            }
            break;

        case ToolMode.References:
            {
                foreach (var searchedAssetInfo in _textureImpl.SearchedAssetInfos)
                {
                    searchedAssetInfo.IsReferences = false;
                }
            }
            break;
        }
    }

    public override bool TrySave() => _textureImpl.TrySave();
}
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
    private int _selectedTextureFormatIdx = Array.FindIndex(AssetImporter_TextureImpl.TextureFormats, _ => _.Equals(TextureImporterFormat.ASTC_6x6.ToString()));
    private int _selectedTextureSizeIdx;
    private int _selectedLabelIdx;
    private int _drawMaxRow = 5;
    private Texture2D _texModified;
    private Vector2 _scrollPos;
    private bool Initialized;
    
    private void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        Initialized = true;

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
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            _selectedTextureFormatIdx = EditorGUILayout.Popup("모든 텍스쳐 압축 포맷 기준", _selectedTextureFormatIdx, AssetImporter_TextureImpl.TextureFormats);
        }
        EditorGUILayout.EndVertical();

        GUIUtil.DrawPopup("텍스쳐 사이즈 검색", ref _selectedTextureSizeIdx, AssetImporter_TextureImpl.TextureSizes, CalcSearchedAssetInfos);
        GUIUtil.DrawPopup("레이블 검색", ref _selectedLabelIdx, _textureImpl.Labels, CalcSearchedAssetInfos);

        void CalcSearchedAssetInfos()
        {
            _textureImpl.CalcSearchedAssetInfos(_selectedLabelIdx, _selectedTextureSizeIdx);
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
        
        if (GUILayout.Button(tex, GUILayout.Width(50), GUILayout.Height(50)))
        {
            AssetImporterTool_TextureWindow.Open(tex);
        }
    }
    
    //TODO: 설명 우선 순위도 결정하게 해주면 좋을듯.
    //TODO: 폰트 크기도 수정해주면 좋을듯.
    private void DrawDesc(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        var tex = assetInfo.Texture2D;
        
        var name = $"Name: {tex.name}";
        var textureType = $"TextureType: {assetInfo.TextureType}";
        var wrapMode = $"WrapMode: {assetInfo.WrapMode}";
        var filterMode = $"FilterMode: {assetInfo.FilterMode}";
        var maxSize = $"Max Size: {assetInfo.MaxTextureSize.ToString()}";
        var format = $"Format: {assetInfo.AOSSettings.format}";
        var textureSize = $"TextureSize: {tex.width}x{tex.height}";
        var fileSize = $"File Size: {assetInfo.FileSize}";
        
        var desc = 
            name + "\n" 
          + textureType + "\n"
          + wrapMode + "\n"
          + filterMode + "\n"
          + maxSize + "\n" 
          + format + "\n"
          + textureSize + "\n"
          + fileSize;
        
        GUILayout.Label(desc, GUIUtil.LabelStyle(TextAnchor.MiddleLeft), GUILayout.Width(280));
    }
    
    private void DrawOption(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        EditorGUILayout.BeginVertical();
        
        Btn("수정", () => AssetImporterTool_Preview.Open(assetInfo));
        Btn("포맷", () => assetInfo.SetPlatformTextureSettings(_selectedTextureFormatIdx));
        Btn("선택", () => Selection.activeObject = assetInfo.Texture2D);
        Btn("열기", () => EditorUtility.RevealInFinder(assetInfo.TextureImporter.assetPath));

        EditorGUILayout.EndVertical();
        
        void Btn(string name, Action act)
        {
            if (GUILayout.Button(name, GUIUtil.ButtonStyle(), GUILayout.Width(50)))
            {
                act();
            }
        }
    }

    public override bool TrySave() => _textureImpl.TrySave(_selectedTextureFormatIdx);
}

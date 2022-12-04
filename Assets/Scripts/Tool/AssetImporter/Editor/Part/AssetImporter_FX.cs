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
    
    private readonly AssetImporter_TextureImpl _originTextureImpl = new();
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

        _originTextureImpl.Initialize();
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
            if (GUILayout.Button("모든 참조 찾기"))
            {
                DependencyImpl.Dependencies(_textureImpl.SearchedAssetInfos);
            }
            if (GUILayout.Button("동일한 텍스쳐 모두 찾기"))
            {
                DependencyImpl.SameAssets(_textureImpl.SearchedAssetInfos);
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
        
        EditorGUILayout.Space(1);
        GUILayout.BeginVertical();
        GUIUtil.Desc("Name", tex.name, keyWidth, valueWidth);
        GUIUtil.Desc("Texture Type", assetInfo.TextureType.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Wrap Mode", assetInfo.WrapMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Filter Mode", assetInfo.FilterMode.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Max Size", assetInfo.MaxTextureSize.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("Format", assetInfo.AOSSettings.format.ToString(), keyWidth, valueWidth);
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

        if (assetInfo.IsReferences)
        {
            Btn("참조", () => AssetImporterTool_References.Open(assetInfo.References));
        }
        if (assetInfo.IsCompare)
        {
            Btn("비교", () => AssetImporterTool_Compare.Open(_compareAssetInfo, assetInfo));
        }

        EditorGUILayout.EndVertical();
        
        void Btn(string name, Action act)
        {
            if (GUILayout.Button(name, GUIUtil.ButtonStyle(), GUILayout.Width(50)))
            {
                act();
            }
        }
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
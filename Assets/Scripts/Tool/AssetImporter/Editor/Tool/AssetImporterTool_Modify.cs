using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal sealed class AssetImporterTool_Modify : EditorWindow
{
    private const float _toolWidth = 400;
    private const float _guiSpace = 10;
    
    private static readonly string[] _textureTypes = Enum.GetNames(typeof(TextureImporterType)).ToArray();
    private int _selectedTextureTypesIdx;
    private int _originTextureTypesIdx;

    private static readonly string[] _wrapModes = Enum.GetNames(typeof(TextureWrapMode)).ToArray();
    private int _selectedWrapModeIdx;
    private int _originWrapModeIdx;
    
    private static readonly string[] _filterModes = Enum.GetNames(typeof(FilterMode)).ToArray();
    private int _selectedFilterModeIdx;
    private int _originFilterModeIdx;
    
    private int _selectedMaxTextureSizeIdx;
    private int _originMaxTextureSizeIdx;

    private int _selectedTextureFormatIdx;
    private int _originTextureFormatIdx;
    
    private AssetImporter_TextureImpl.AssetInfo _assetInfo;
    
    public static void Open(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetImporterTool_Modify>("Modify");
        tool.minSize = tool.maxSize = new Vector2(_toolWidth, 540);
        tool._assetInfo = assetInfo;

        SetOption(tool);
    }

    private static void SetOption(AssetImporterTool_Modify tool)
    {
        var assetInfo = tool._assetInfo;
        
        tool._originTextureTypesIdx = tool._selectedTextureTypesIdx = Array.FindIndex(_textureTypes, _ => _.Equals(assetInfo.TextureType.ToString()));
        tool._originWrapModeIdx = tool._selectedWrapModeIdx = Array.FindIndex(_wrapModes, _ => _.Equals(assetInfo.WrapMode.ToString()));
        tool._originFilterModeIdx = tool._selectedFilterModeIdx = Array.FindIndex(_filterModes, _ => _.Equals(assetInfo.FilterMode.ToString()));
        tool._originMaxTextureSizeIdx = tool._selectedMaxTextureSizeIdx = Array.FindIndex(AssetImporter_TextureImpl.TextureSizes, _ => _.Equals(assetInfo.MaxTextureSize.ToString()));
        tool._originTextureFormatIdx = tool._selectedTextureFormatIdx = Array.FindIndex(AssetImporter_TextureImpl.TextureFormats, _ => _.Equals(assetInfo.AOSSettings.format.ToString()));
    }

    private void ReSetOption()
    {
        _selectedTextureTypesIdx = _originTextureTypesIdx;
        _selectedWrapModeIdx = _originWrapModeIdx;
        _selectedFilterModeIdx = _originFilterModeIdx;
        _selectedMaxTextureSizeIdx = _originMaxTextureSizeIdx;
        _selectedTextureFormatIdx = _originTextureFormatIdx;
        _assetInfo.FileSize = EditorTextureUtil.TextureSize(_assetInfo.Texture2D);
    }
    
    private void OnDisable()
    {
        if (_assetInfo.Changed == false)
        {
            _assetInfo.FileSize = EditorTextureUtil.TextureSize(_assetInfo.Texture2D);
        }
    }
    
    private void OnGUI()
    {
        DrawTexture();
        DrawOption();
        DrawMenus();
    }
    
    private void DrawMenus()
    {
        const float width = 100;
        
        EditorGUILayout.Space(_guiSpace);
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.BtnExpand("선택", width, () => Selection.activeObject = _assetInfo.Texture2D);
            GUIUtil.BtnExpand("열기", width, () => EditorUtility.RevealInFinder(_assetInfo.TextureImporter.assetPath));
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.BtnExpand("저장", width, Save);
            GUIUtil.BtnExpand("설정 값 되돌리기", width, ReSetOption);
        }
        EditorGUILayout.EndHorizontal();
        
        GUIUtil.BtnExpand("압축 포맷 별로 보기", width, () => AssetImporterTool_Format.Open(_assetInfo));
    }
    
    private void DrawTexture()
    {
        var tex = _assetInfo.Texture2D;
        var importer = _assetInfo.TextureImporter;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(_guiSpace);
        {
            GUILayout.Label(tex, GUIUtil.LabelStyle(), GUILayout.Width(_toolWidth), GUILayout.Height(256));
        }
        EditorGUILayout.Space(_guiSpace);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            GUILayout.Label($"{importer.assetPath} ({_assetInfo.FileSize})", GUIUtil.LabelStyle(TextAnchor.MiddleLeft));
            GUILayout.Label(tex.name, GUIUtil.LabelStyle(TextAnchor.MiddleLeft));
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawOption()
    {
        GUIUtil.DrawPopup("Texture Type", ref _selectedTextureTypesIdx, _textureTypes);
        GUIUtil.DrawPopup("Wrap Mode", ref _selectedWrapModeIdx, _wrapModes);
        GUIUtil.DrawPopup("Filter Mode", ref _selectedFilterModeIdx, _filterModes);
        GUIUtil.DrawPopup("Max Size", ref _selectedMaxTextureSizeIdx, AssetImporter_TextureImpl.TextureSizes, () => _assetInfo.AOSSettings.overridden = true);
        GUIUtil.DrawPopup("Format", ref _selectedTextureFormatIdx, AssetImporter_TextureImpl.TextureFormats, () => _assetInfo.AOSSettings.overridden = true); 
    }
    
    private void Save()
    {
        _assetInfo.Changed = _selectedTextureTypesIdx != _originTextureTypesIdx
                          || _selectedWrapModeIdx != _originWrapModeIdx
                          || _selectedFilterModeIdx != _originFilterModeIdx
                          || _selectedMaxTextureSizeIdx != _originMaxTextureSizeIdx
                          || _selectedTextureFormatIdx != _originTextureFormatIdx;

        if (_assetInfo.Changed == false)
        {
            EditorUtility.DisplayDialog("알림", "변경된 사항이 없습니다.", "확인");
            return;
        }
        
        _assetInfo.TextureType = Enum.Parse<TextureImporterType>(_textureTypes[_selectedTextureTypesIdx]);
        _assetInfo.WrapMode = Enum.Parse<TextureWrapMode>(_wrapModes[_selectedWrapModeIdx]);
        _assetInfo.FilterMode = Enum.Parse<FilterMode>(_filterModes[_selectedFilterModeIdx]);
        _assetInfo.MaxTextureSize = int.Parse(AssetImporter_TextureImpl.TextureSizes[_selectedMaxTextureSizeIdx]);
   
        if (_selectedTextureFormatIdx > -1)
        {
            var formatStr = AssetImporter_TextureImpl.TextureFormats[_selectedTextureFormatIdx];
            var format = Enum.Parse<TextureImporterFormat>(formatStr);
            _assetInfo.SetTextureImporterFormat(format, true);
        }
        
        Close();
    }
}
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal sealed class AssetManagementTool_TextureModify : EditorWindow
{
    private const float TOOL_WIDTH = 400;
    private const float GUI_SPACE = 10;
    
    private static readonly string[] _textureTypes = Enum.GetNames(typeof(TextureImporterType)).ToArray();
    private int _selectedTextureTypesIdx;
    private int _originTextureTypesIdx;

    private static readonly string[] _wrapModes = Enum.GetNames(typeof(TextureWrapMode)).ToArray();
    private int _selectedWrapModeIdx;
    private int _originWrapModeIdx;
    
    private static readonly string[] _filterModes = Enum.GetNames(typeof(FilterMode)).ToArray();
    private int _selectedFilterModeIdx;
    private int _originFilterModeIdx;

    private static readonly string[] _textureSize =
    {
        "2048", 
        "1024", 
        "512", 
        "256", 
        "128", 
        "64", 
        "32",
    };
    private int _selectedMaxTextureSizeIdx;
    private int _originMaxTextureSizeIdx;

    private int _selectedTextureFormatIdx;
    private int _originTextureFormatIdx;
    
    private AssetManagementImpl_Texture.AssetInfo _assetInfo;
    
    public static void Open(AssetManagementImpl_Texture.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetManagementTool_TextureModify>("Modify");
        tool.minSize = tool.maxSize = new Vector2(TOOL_WIDTH, 530);
        tool._assetInfo = assetInfo;

        SetOption(tool);
    }

    private static void SetOption(AssetManagementTool_TextureModify tool)
    {
        var assetInfo = tool._assetInfo;
        
        tool._originTextureTypesIdx = tool._selectedTextureTypesIdx = Array.FindIndex(_textureTypes, _ => _.Equals(assetInfo.TextureType.ToString()));
        tool._originWrapModeIdx = tool._selectedWrapModeIdx = Array.FindIndex(_wrapModes, _ => _.Equals(assetInfo.WrapMode.ToString()));
        tool._originFilterModeIdx = tool._selectedFilterModeIdx = Array.FindIndex(_filterModes, _ => _.Equals(assetInfo.FilterMode.ToString()));
        tool._originMaxTextureSizeIdx = tool._selectedMaxTextureSizeIdx = Array.FindIndex(AssetManagementImpl_Texture.TextureSizes, _ => _.Equals(assetInfo.MaxTextureSize.ToString()));
        tool._originTextureFormatIdx = tool._selectedTextureFormatIdx = Array.FindIndex(AssetManagementImpl_Texture.TextureFormats, _ => _.Equals(assetInfo.AOSSettings.format.ToString()));
    }

    private void ReSetOption()
    {
        _selectedTextureTypesIdx = _originTextureTypesIdx;
        _selectedWrapModeIdx = _originWrapModeIdx;
        _selectedFilterModeIdx = _originFilterModeIdx;
        _selectedMaxTextureSizeIdx = _originMaxTextureSizeIdx;
        _selectedTextureFormatIdx = _originTextureFormatIdx;
        _assetInfo.FileSizeStr = EditorTextureUtil.TextureSize(_assetInfo.Texture2D);
    }
    
    private void OnDisable()
    {
        if (!_assetInfo.Changed)
        {
            _assetInfo.FileSizeStr = EditorTextureUtil.TextureSize(_assetInfo.Texture2D);
        }
    }
    
    private void OnGUI()
    {
        DrawTexture();
        DrawOption();
        DrawMenus();
    }
    
    private void DrawTexture()
    {
        var tex = _assetInfo.Texture2D;
        var importer = _assetInfo.TextureImporter;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(GUI_SPACE);
        {
            GUILayout.Label(tex, GUIUtil.LabelStyle(), GUILayout.Width(TOOL_WIDTH), GUILayout.Height(256));
        }
        EditorGUILayout.Space(GUI_SPACE);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            GUILayout.Label($"{importer.assetPath} ({_assetInfo.FileSizeStr})", GUIUtil.LabelStyle(TextAnchor.MiddleLeft));
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawOption()
    {
        GUIUtil.DrawPopup("Texture Type", ref _selectedTextureTypesIdx, _textureTypes);
        GUIUtil.DrawPopup("Wrap Mode", ref _selectedWrapModeIdx, _wrapModes);
        GUIUtil.DrawPopup("Filter Mode", ref _selectedFilterModeIdx, _filterModes);
        GUIUtil.DrawPopup("Max Size", ref _selectedMaxTextureSizeIdx, _textureSize, () => _assetInfo.AOSSettings.overridden = true);
        GUIUtil.DrawPopup("Format", ref _selectedTextureFormatIdx, AssetManagementImpl_Texture.TextureFormats, () => _assetInfo.AOSSettings.overridden = true); 
    }
    
    private void DrawMenus()
    {
        const float width = 100;
        
        EditorGUILayout.Space(GUI_SPACE);
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
        
        GUIUtil.BtnExpand("압축 포맷 별로 보기", width, () => AssetManagementTool_Format.Open(_assetInfo));
    }
    
    private void Save()
    {
        var changed = IsChanged();
        if (!changed)
        {
            EditorUtility.DisplayDialog("알림", "변경된 사항이 없습니다.", "확인");
            return;
        }

        changed = !IsOriginChanged();
        if (!changed)
        {
            EditorUtility.DisplayDialog("알림", "변경전 사항과 동일합니다.", "확인");
            return;
        }

        _assetInfo.Changed = true;
        _assetInfo.TextureType = Enum.Parse<TextureImporterType>(_textureTypes[_selectedTextureTypesIdx]);
        _assetInfo.WrapMode = Enum.Parse<TextureWrapMode>(_wrapModes[_selectedWrapModeIdx]);
        _assetInfo.FilterMode = Enum.Parse<FilterMode>(_filterModes[_selectedFilterModeIdx]);
        _assetInfo.MaxTextureSize = int.Parse(AssetManagementImpl_Texture.TextureSizes[_selectedMaxTextureSizeIdx]);
        _assetInfo.ForceSetTextureImporterFormat(_selectedTextureFormatIdx);
        
        Close();
    }

    private bool IsChanged()
    {
        return _selectedTextureTypesIdx != _originTextureTypesIdx 
            || _selectedWrapModeIdx != _originWrapModeIdx
            || _selectedFilterModeIdx != _originFilterModeIdx 
            || _selectedMaxTextureSizeIdx != _originMaxTextureSizeIdx 
            || _selectedTextureFormatIdx != _originTextureFormatIdx;
    }

    private bool IsOriginChanged()
    {
        var textureImporter = _assetInfo.TextureImporter;
        var textureType = Enum.Parse<TextureImporterType>(_textureTypes[_selectedTextureTypesIdx]);
        var wrapMode = Enum.Parse<TextureWrapMode>(_wrapModes[_selectedWrapModeIdx]);
        var filterMode = Enum.Parse<FilterMode>(_filterModes[_selectedFilterModeIdx]);
        var maxTextureSize = int.Parse(AssetManagementImpl_Texture.TextureSizes[_selectedMaxTextureSizeIdx]);

        if (_selectedTextureFormatIdx < 0)
        {
            return textureType == textureImporter.textureType
                && wrapMode == textureImporter.wrapMode
                && filterMode == textureImporter.filterMode
                && maxTextureSize == textureImporter.maxTextureSize;
        }
        
        var formatStr = AssetManagementImpl_Texture.TextureFormats[_selectedTextureFormatIdx];
        var format = Enum.Parse<TextureImporterFormat>(formatStr);
        
        return textureType == textureImporter.textureType
            && wrapMode == textureImporter.wrapMode
            && filterMode == textureImporter.filterMode
            && maxTextureSize == textureImporter.maxTextureSize
            && format == _assetInfo.FormatType;
    }
}
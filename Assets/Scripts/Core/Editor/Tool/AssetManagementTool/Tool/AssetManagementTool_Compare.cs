using UnityEditor;
using UnityEngine;

public sealed class AssetManagementTool_Compare : EditorWindow
{
    private sealed class TextureInfo
    {
        public Texture Tex { get; }
        public TextureImporter Importer { get; }
        public TextureImporterPlatformSettings Settings { get; }
        public string FileSize { get; }
        
        public TextureInfo(Texture tex)
        {
            Tex = tex;
            Importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
            Settings = Importer.GetPlatformTextureSettings("Android");
            FileSize = EditorTextureUtil.TextureSize(Tex);
        }
    }
    
    private const float _keyWidth = 80;
    private const float _valueWidth = 170;
    private const float _textureSize = 200;
    private TextureInfo _left, _right;
    
    public static void Open(Texture left, Texture right)
    {
        var tool = GetWindow<AssetManagementTool_Compare>("Compare");
        tool.minSize = tool.maxSize = new Vector2(_textureSize * 3, _textureSize * 2);
        tool._left = new TextureInfo(left);
        tool._right = new TextureInfo(right);
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        Set(_left);
        EditorGUILayout.Space(50);
        Set(_right);
        EditorGUILayout.EndHorizontal();
    }
    
    private void Set(TextureInfo textureInfo)
    {
        var tex = textureInfo.Tex;
        var importer = textureInfo.Importer;
        var setting = textureInfo.Settings;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIUtil.Btn(tex, _textureSize + 60, _textureSize, () => Selection.activeObject = tex);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIUtil.Desc($"{importer.assetPath}");
        GUIUtil.Desc(tex.name);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIUtil.Desc("Texture Type", importer.textureType.ToString(), _keyWidth, _valueWidth, _left.Importer.textureType, _right.Importer.textureType);
        GUIUtil.Desc("Wrap Mode", tex.wrapMode.ToString(), _keyWidth, _valueWidth, _left.Tex.wrapMode, _right.Tex.wrapMode);
        GUIUtil.Desc("Filter Mode", tex.filterMode.ToString(), _keyWidth, _valueWidth, _left.Tex.filterMode, _right.Tex.filterMode);
        GUIUtil.Desc("Max Size", importer.maxTextureSize.ToString(), _keyWidth, _valueWidth, _left.Importer.maxTextureSize, _right.Importer.maxTextureSize);
        GUIUtil.Desc("Format", setting.format.ToString(), _keyWidth, _valueWidth, _left.Settings.format, _right.Settings.format);
        GUIUtil.Desc("Texture Size", $"{tex.width.ToString()}x{tex.height.ToString()}", _keyWidth, _valueWidth);
        GUIUtil.Desc("File Size", textureInfo.FileSize, _keyWidth, _valueWidth);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginHorizontal();
        GUIUtil.Btn("선택", () => Selection.activeObject = tex);
        GUIUtil.Btn("열기", () => EditorUtility.RevealInFinder(importer.assetPath));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
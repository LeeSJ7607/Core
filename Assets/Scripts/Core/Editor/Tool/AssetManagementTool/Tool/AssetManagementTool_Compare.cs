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
    
    private const float KEY_WIDTH = 80;
    private const float VALUE_WIDTH = 170;
    private const float TEXTURE_SIZE = 200;
    private TextureInfo _left, _right;
    
    public static void Open(Texture left, Texture right)
    {
        var tool = GetWindow<AssetManagementTool_Compare>("Compare");
        tool.minSize = tool.maxSize = new Vector2(TEXTURE_SIZE * 3, TEXTURE_SIZE * 2);
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
        GUIUtil.Btn(tex, TEXTURE_SIZE + 60, TEXTURE_SIZE, () => Selection.activeObject = tex);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIUtil.Desc($"{importer.assetPath}");
        GUIUtil.Desc(tex.name);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIUtil.Desc("Texture Type", importer.textureType.ToString(), KEY_WIDTH, VALUE_WIDTH, _left.Importer.textureType, _right.Importer.textureType);
        GUIUtil.Desc("Wrap Mode", tex.wrapMode.ToString(), KEY_WIDTH, VALUE_WIDTH, _left.Tex.wrapMode, _right.Tex.wrapMode);
        GUIUtil.Desc("Filter Mode", tex.filterMode.ToString(), KEY_WIDTH, VALUE_WIDTH, _left.Tex.filterMode, _right.Tex.filterMode);
        GUIUtil.Desc("Max Size", importer.maxTextureSize.ToString(), KEY_WIDTH, VALUE_WIDTH, _left.Importer.maxTextureSize, _right.Importer.maxTextureSize);
        GUIUtil.Desc("Format", setting.format.ToString(), KEY_WIDTH, VALUE_WIDTH, _left.Settings.format, _right.Settings.format);
        GUIUtil.Desc("Texture Size", $"{tex.width.ToString()}x{tex.height.ToString()}", KEY_WIDTH, VALUE_WIDTH);
        GUIUtil.Desc("File Size", textureInfo.FileSize, KEY_WIDTH, VALUE_WIDTH);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginHorizontal();
        GUIUtil.Btn("선택", () => Selection.activeObject = tex);
        GUIUtil.Btn("열기", () => EditorUtility.RevealInFinder(importer.assetPath));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
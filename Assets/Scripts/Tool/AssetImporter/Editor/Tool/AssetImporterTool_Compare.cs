using System;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_Compare : EditorWindow
{
    private const float _keyWidth = 80;
    private const float _valueWidth = 170;
    private const float _textureSize = 200;
    private AssetImporter_TextureImpl.AssetInfo _left;
    private AssetImporter_TextureImpl.AssetInfo _right;
    
    public static void Open(AssetImporter_TextureImpl.AssetInfo left, AssetImporter_TextureImpl.AssetInfo right)
    {
        var tool = GetWindow<AssetImporterTool_Compare>("Compare");
        tool.minSize = tool.maxSize = new Vector2(_textureSize * 3, _textureSize * 2);
        tool._left = left;
        tool._right = right;
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        Set(_left);
        EditorGUILayout.Space(50);
        Set(_right);
        EditorGUILayout.EndHorizontal();
    }
    
    private void Desc(string key, string value, object lValue, object rValue)
    {
        if (lValue.Equals(rValue))
        {
            GUIUtil.Desc(key, value, _keyWidth, _valueWidth);
        }
        else
        {
            GUIUtil.DescColor(key, value, _keyWidth, _valueWidth);
        }
    }
    
    private void Set(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        var tex = assetInfo.Texture2D;
        var importer = assetInfo.TextureImporter;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (GUILayout.Button(tex, GUIUtil.ButtonStyle(), GUILayout.Width(_textureSize + 60), GUILayout.Height(_textureSize)))
        {
            
        }
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUIUtil.Desc($"{importer.assetPath} ({assetInfo.FileSize})");
        GUIUtil.Desc(tex.name);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        Desc("Texture Type", assetInfo.TextureType.ToString(), _left.TextureType, _right.TextureType);
        Desc("Wrap Mode", assetInfo.WrapMode.ToString(), _left.WrapMode, _right.WrapMode);
        Desc("Filter Mode", assetInfo.FilterMode.ToString(), _left.FilterMode, _right.FilterMode);
        Desc("Max Size", importer.maxTextureSize.ToString(), _left.TextureImporter.maxTextureSize, _right.TextureImporter.maxTextureSize);
        Desc("Format", assetInfo.AOSSettings.format.ToString(), _left.AOSSettings.format, _right.AOSSettings.format);
        GUIUtil.Desc("Texture Size", $"{tex.width.ToString()}x{tex.height.ToString()}", _keyWidth, _valueWidth);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        Btn("선택", () => Selection.activeObject = assetInfo.Texture2D);
        Btn("열기", () => EditorUtility.RevealInFinder(assetInfo.TextureImporter.assetPath));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
        void Btn(string name, Action act)
        {
            if (GUILayout.Button(name, GUIUtil.ButtonStyle(), GUILayout.ExpandWidth(true)))
            {
                act();
            }
        }
    }
}
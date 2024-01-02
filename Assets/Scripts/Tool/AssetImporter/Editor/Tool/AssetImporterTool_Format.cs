using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal sealed class AssetImporterTool_Format : EditorWindow
{
    private const float _size = 200;
    
    private sealed class TextureInfo
    {
        public string Path { get; }
        public Texture2D Tex { get; }
        public TextureImporterFormat Format { get; }
        public string Size { get; }

        public TextureInfo(string path, TextureImporterFormat format)
        {
            Path = path;
            Tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Format = format;
            Size = EditorTextureUtil.TextureSize(Tex);
        }
    }
    
    private readonly List<TextureInfo> _textureInfos = new();

    public static void Open(AssetImporterImpl_Texture.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetImporterTool_Format>("Format");
        tool.minSize = tool.maxSize = new Vector2(_size * AssetImporterImpl_Texture.TextureFormats.Length + 40, _size + 40);
        
        CreateTexture(tool, assetInfo);
    }
    
    private void OnDisable()
    {
        GetWindow<AssetImporterTool_Preview>().Close();
        
        foreach (var textureInfo in _textureInfos)
        {
            AssetDatabase.DeleteAsset(textureInfo.Path);
        }
    }

    private static void CreateTexture(AssetImporterTool_Format tool, AssetImporterImpl_Texture.AssetInfo assetInfo)
    {
        var path = AssetDatabase.GetAssetPath(assetInfo.Texture2D);
        
        for (var i = 0; i < AssetImporterImpl_Texture.TextureFormats.Length; i++)
        {
            var newPath = $"{path}{i.ToString()}.png";
            AssetDatabase.CopyAsset(path, newPath);
        
            var importer = (TextureImporter)UnityEditor.AssetImporter.GetAtPath(newPath);
            var settings = importer.GetPlatformTextureSettings("Android");
            settings.overridden = true;
            settings.format = Enum.Parse<TextureImporterFormat>(AssetImporterImpl_Texture.TextureFormats[i]);
            importer.SetPlatformTextureSettings(settings);
            importer.SaveAndReimport();
            
            tool._textureInfos.Add(new TextureInfo(newPath, settings.format));
        }
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        
        foreach (var textureInfo in _textureInfos)
        {
            EditorGUILayout.BeginVertical();
            
            DrawTexture(textureInfo);
            DrawDesc(textureInfo);
            
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawTexture(TextureInfo textureInfo)
    {
        var tex = textureInfo.Tex;
        GUIUtil.Btn(tex, _size, _size, () => AssetImporterTool_Preview.Open(tex));
    }

    private void DrawDesc(TextureInfo textureInfo)
    {
        var desc = $"{textureInfo.Format}\n{textureInfo.Size}";
        GUILayout.Label(desc, GUIUtil.LabelStyle(), GUILayout.Width(_size));
    }
}
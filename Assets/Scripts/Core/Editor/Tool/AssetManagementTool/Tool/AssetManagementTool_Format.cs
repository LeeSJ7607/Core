using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal sealed class AssetManagementTool_Format : EditorWindow
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

    public static void Open(AssetManagementImpl_Texture.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetManagementTool_Format>("Format");
        tool.minSize = tool.maxSize = new Vector2(_size * AssetManagementImpl_Texture.TextureFormats.Length + 40, _size + 40);
        
        CreateTexture(tool, assetInfo);
    }
    
    private void OnDisable()
    {
        GetWindow<AssetManagementTool_Preview>().Close();
        
        foreach (var textureInfo in _textureInfos)
        {
            AssetDatabase.DeleteAsset(textureInfo.Path);
        }
    }

    private static void CreateTexture(AssetManagementTool_Format tool, AssetManagementImpl_Texture.AssetInfo assetInfo)
    {
        var path = AssetDatabase.GetAssetPath(assetInfo.Texture2D);
        
        for (var i = 0; i < AssetManagementImpl_Texture.TextureFormats.Length; i++)
        {
            var newPath = $"{path}{i.ToString()}.png";
            AssetDatabase.CopyAsset(path, newPath);
        
            var importer = (TextureImporter)AssetImporter.GetAtPath(newPath);
            var settings = importer.GetPlatformTextureSettings("Android");
            settings.overridden = true;
            settings.format = Enum.Parse<TextureImporterFormat>(AssetManagementImpl_Texture.TextureFormats[i]);
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
        GUIUtil.Btn(tex, _size, _size, () => AssetManagementTool_Preview.Open(tex));
    }

    private void DrawDesc(TextureInfo textureInfo)
    {
        var desc = $"{textureInfo.Format}\n{textureInfo.Size}";
        GUILayout.Label(desc, GUIUtil.LabelStyle(), GUILayout.Width(_size));
    }
}
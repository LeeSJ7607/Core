using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporter_TextureImpl
{
    private const string _noneLabel = "None";
    
    public static readonly string[] TextureFormats = 
    {
        TextureImporterFormat.ASTC_12x12.ToString(),
        TextureImporterFormat.ASTC_10x10.ToString(),
        TextureImporterFormat.ASTC_8x8.ToString(),
        TextureImporterFormat.ASTC_6x6.ToString(),
        TextureImporterFormat.ASTC_5x5.ToString(),
        TextureImporterFormat.ASTC_4x4.ToString()
    };

    public static readonly string[] TextureSizes =
    {
        "2048", 
        "1024", 
        "512", 
        "256", 
        "128", 
        "64", 
        "32",
        "0"
    };

    public string[] Labels { get; private set; }

    public sealed class AssetInfo
    {
        public string Path { get; }
        public Texture2D Texture2D { get; private set; }
        public TextureImporter TextureImporter { get; }
        public TextureImporterPlatformSettings AOSSettings { get; }
        public TextureImporterType TextureType { get; set; }
        public TextureWrapMode WrapMode { get; set; }
        public FilterMode FilterMode { get; set; }
        public int MaxTextureSize { get; set; }
        public string FileSize { get; set; }
        public bool Changed { get; set; }

        public AssetInfo(string guid)
        {
            Path = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(Path);
            TextureImporter = (TextureImporter)AssetImporter.GetAtPath(Path);
            AOSSettings = TextureImporter.GetPlatformTextureSettings("Android");
            TextureType = TextureImporter.textureType;
            WrapMode = TextureImporter.wrapMode;
            FilterMode = TextureImporter.filterMode;
            MaxTextureSize = TextureImporter.maxTextureSize;
            FileSize = EditorTextureUtil.TextureSize(Texture2D);
        }

        private void Refresh()
        {
            Texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(Path);
            FileSize = EditorTextureUtil.TextureSize(Texture2D);
            Changed = false;
        }
        
        private void SetPlatformTextureSettings()
        {
            var ios = TextureImporter.GetPlatformTextureSettings("iPhone");
            ios.maxTextureSize = AOSSettings.maxTextureSize;
            ios.format = AOSSettings.format;
            
            TextureImporter.SetPlatformTextureSettings(AOSSettings);
            TextureImporter.SetPlatformTextureSettings(ios);
            TextureImporter.SaveAndReimport();
        }

        public void SetPlatformTextureSettings(int format)
        {
            AOSSettings.format = Enum.Parse<TextureImporterFormat>(TextureFormats[format]);
            Changed = true;
        }

        public void Save()
        {
            SetPlatformTextureSettings();
            Refresh();
        }
    }
    
    public List<AssetInfo> SearchedAssetInfos { get; } = new();
    private readonly List<AssetInfo> _assetInfos = new();
    private string _curRootFindAssets = "Assets/Temp";
    private bool Initialized;

    public void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        Initialized = true;
        
        //TODO: 저장된 포맷을 적용한다.
        //_selectedTextureFormatIdx
        
        //TODO: 저장된 경로를 적용한다.
        //_curRootFindAssets
        
        CreateLabelsAndAssets();
    }

    private void CreateLabelsAndAssets()
    {
        var guids = AssetDatabase.FindAssets("t:texture", new[] { _curRootFindAssets });
        
        CreateLabels(guids);
        CreateAssets(guids);
    }
    
    private void CreateLabels(IEnumerable<string> guids)
    {
        var str = new List<string> { _noneLabel };

        foreach (var guid in guids)
        {
            str.AddRange(AssetDatabase.GetLabels(new GUID(guid)));
        }

        Labels = str.ToArray();
    }
    
    private void CreateAssets(IEnumerable<string> guids)
    {
        foreach (var guid in guids)
        {
            var assetInfo = new AssetInfo(guid);
            
            _assetInfos.Add(assetInfo);
            SearchedAssetInfos.Add(assetInfo);
        }
    }
    
    public void CalcSearchedAssetInfos(int selectedLabelIdx, int selectedTextureMaxSizeIdx, int selectedTextureMinSizeIdx)
    {
        SearchedAssetInfos.Clear();

        foreach (var assetInfo in _assetInfos)
        {
            var tex = assetInfo.Texture2D;
            var label = Labels[selectedLabelIdx];
            var textureMaxSize = int.Parse(TextureSizes[selectedTextureMaxSizeIdx]);
            var textureMinSize = int.Parse(TextureSizes[selectedTextureMinSizeIdx]);

            var existLabel = ExistLabel(tex, label);
            var checkSizeTexture = CheckSizeTexture(tex, textureMaxSize, textureMinSize);

            if (label.Equals(_noneLabel) == false && textureMaxSize > 0)
            {
                if (existLabel && checkSizeTexture)
                {
                    SearchedAssetInfos.Add(assetInfo);
                }
                continue;
            }

            if (textureMaxSize > 0)
            {
                if (checkSizeTexture)
                {
                    SearchedAssetInfos.Add(assetInfo);
                }
            }
            else
            {
                if (existLabel)
                {
                    SearchedAssetInfos.Add(assetInfo);
                }
            }
        }
        
        bool ExistLabel(Texture2D tex, string label) => AssetDatabase.GetLabels(tex).Contains(label);
        bool CheckSizeTexture(Texture2D tex, int maxSize, int minSize)
        {
            return tex.width <= maxSize && tex.height <= maxSize
                && tex.width >= minSize && tex.height >= minSize;
        }
    }

    public bool TrySave()
    {
        var changed = false;
        UnityEngine.Object activeObject = null;

        foreach (var assetInfo in _assetInfos)
        {
            if (assetInfo.Changed == false)
            {
                continue;
            }
            
            var importer = assetInfo.TextureImporter;
            importer.textureType = assetInfo.TextureType;
            importer.wrapMode = assetInfo.WrapMode;
            importer.filterMode = assetInfo.FilterMode;
            importer.maxTextureSize = assetInfo.MaxTextureSize;
            assetInfo.Save();
            changed = true;

            activeObject = assetInfo.Texture2D;
        }

        if (changed)
        {
            Selection.activeObject = activeObject;
        }
        
        return changed;
    }
}
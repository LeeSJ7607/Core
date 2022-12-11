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
        private readonly string _path;
        public Texture2D Texture2D { get; private set; }
        public TextureImporter TextureImporter { get; }
        public TextureImporterPlatformSettings AOSSettings { get; }
        public TextureImporterType TextureType { get; set; }
        public TextureWrapMode WrapMode { get; set; }
        public FilterMode FilterMode { get; set; }
        public int MaxTextureSize { get; set; }
        public string FileSize { get; set; }
        public IReadOnlyDictionary<UnityEngine.Object, IReadOnlyList<UnityEngine.Object>> References { get; set; } 
        public bool IsReferences { get; set; }
        public IReadOnlyDictionary<int, DependencyImpl.SameAssetInfo> Compares { get; set; }
        public bool IsCompare { get; set; }
        public bool Changed { get; set; }

        public AssetInfo(string guid)
        {
            _path = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(_path);
            TextureImporter = (TextureImporter)AssetImporter.GetAtPath(_path);
            AOSSettings = TextureImporter.GetPlatformTextureSettings("Android");
            TextureType = TextureImporter.textureType;
            WrapMode = TextureImporter.wrapMode;
            FilterMode = TextureImporter.filterMode;
            MaxTextureSize = TextureImporter.maxTextureSize;
            FileSize = EditorTextureUtil.TextureSize(Texture2D);
        }

        private void Refresh()
        {
            Texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(_path);
            FileSize = EditorTextureUtil.TextureSize(Texture2D);
            Changed = false;
        }

        private void SetTextureImporter()
        {
            var importer = TextureImporter;
            importer.textureType = TextureType;
            importer.wrapMode = WrapMode;
            importer.filterMode = FilterMode;
            importer.maxTextureSize = MaxTextureSize;
        }
        
        private void SetPlatformTextureSettings()
        {
            var ios = TextureImporter.GetPlatformTextureSettings("iPhone");
            ios.overridden = AOSSettings.overridden = true;
            ios.maxTextureSize = MaxTextureSize;
            ios.format = AOSSettings.format;
            
            TextureImporter.SetPlatformTextureSettings(AOSSettings);
            TextureImporter.SetPlatformTextureSettings(ios);
            TextureImporter.SaveAndReimport();
        }

        public void SetTextureImporterFormat(int formatIdx)
        {
            var format = Enum.Parse<TextureImporterFormat>(TextureFormats[formatIdx]);
            if (AOSSettings.format == format)
            {
                return;
            }

            AOSSettings.format = format;
            Changed = true;
        }

        public void ReSetTextureImporterFormat()
        {
            AOSSettings.format = TextureImporter.GetPlatformTextureSettings("Android").format;
            Changed = false;
        }

        public void Save()
        {
            SetTextureImporter();
            SetPlatformTextureSettings();
            Refresh();
        }
    }
    
    public (SortTexture sortType, bool descending) CurSort { private get; set; }
    public IReadOnlyList<AssetInfo> SearchedAssetInfos => _searchedAssetInfos;
    private List<AssetInfo> _searchedAssetInfos = new();
    public IReadOnlyList<AssetInfo> AssetInfos => _assetInfos;
    private readonly List<AssetInfo> _assetInfos = new();
    private string _curRootFindAssets = "Assets/Temp";
    private bool _initialized;

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        
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
            _searchedAssetInfos.Add(assetInfo);
        }
    }

    public void CalcSearchedAssetInfos(
        int selectedLabelIdx, 
        int selectedTextureMaxSizeIdx, 
        int selectedTextureMinSizeIdx,
        string searchedTextureName)
    {
        _searchedAssetInfos.Clear();

        foreach (var assetInfo in _assetInfos)
        {
            var tex = assetInfo.Texture2D;
            if (SearchedTextureName(tex.name) == false)
            {
                continue;
            }
            
            var label = Labels[selectedLabelIdx];
            var textureMaxSize = int.Parse(TextureSizes[selectedTextureMaxSizeIdx]);
            var textureMinSize = int.Parse(TextureSizes[selectedTextureMinSizeIdx]);
            var existLabel = ExistLabel(tex, label);
            var checkSizeTexture = CheckSizeTexture(tex, textureMaxSize, textureMinSize);

            if (label.Equals(_noneLabel) == false && textureMaxSize > 0)
            {
                if (existLabel && checkSizeTexture)
                {
                    _searchedAssetInfos.Add(assetInfo);
                }
                continue;
            }

            if (textureMaxSize > 0)
            {
                if (checkSizeTexture)
                {
                    _searchedAssetInfos.Add(assetInfo);
                }
            }
            else
            {
                if (existLabel)
                {
                    _searchedAssetInfos.Add(assetInfo);
                }
            }
        }

        Sort();
        
        bool ExistLabel(Texture2D tex, string label) => AssetDatabase.GetLabels(tex).Contains(label);
        bool SearchedTextureName(string name)
        {
            return string.IsNullOrWhiteSpace(searchedTextureName) 
                || name.ToLower().Contains(searchedTextureName.ToLower());
        }
        bool CheckSizeTexture(Texture2D tex, int maxSize, int minSize)
        {
            return tex.width <= maxSize && tex.height <= maxSize
                && tex.width >= minSize && tex.height >= minSize;
        }
    }
    
    private void Sort()
    {
        switch (CurSort.sortType)
        {
        case SortTexture.Name:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.Texture2D.name).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.Texture2D.name).ToList();
            }
            break;

        case SortTexture.FileSize:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.FileSize).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.FileSize).ToList();
            }
            break;
        
        case SortTexture.TextureSize:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.Texture2D.width).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.Texture2D.width).ToList();
            }
            break;
        
        case SortTexture.MipMap:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.TextureImporter.mipmapEnabled).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.TextureImporter.mipmapEnabled).ToList();
            }
            break;
        
        case SortTexture.Format:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.AOSSettings.format).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.AOSSettings.format).ToList();
            }
            break;
        
        case SortTexture.WrapMode:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.WrapMode).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.WrapMode).ToList();
            }
            break;
        
        case SortTexture.FilterMode:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.FilterMode).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.FilterMode).ToList();
            }
            break;
        
        case SortTexture.TextureType:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.TextureType).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.TextureType).ToList();
            }
            break;
        
        case SortTexture.References:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.IsReferences).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.IsReferences).ToList();
            }
            break;
        
        case SortTexture.Compare:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.IsCompare).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.IsCompare).ToList();
            }
            break;
        }
    }
    
    public bool CanDiff()
    {
        foreach (var assetInfo in _assetInfos)
        {
            if (assetInfo.Changed)
            {
                return true;
            }
        }

        return false;
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
            
            activeObject = assetInfo.Texture2D;
            assetInfo.Save();
            changed = true;
        }

        if (changed)
        {
            Selection.activeObject = activeObject;
        }
        
        return changed;
    }
}
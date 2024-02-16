using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class AssetImporterImpl_Texture : IAssetImporterImpl
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
        public TextureImporterFormat FormatType { get; private set; }
        public TextureImporterType TextureType { get; set; }
        public TextureWrapMode WrapMode { get; set; }
        public FilterMode FilterMode { get; set; }
        public int MaxTextureSize { get; set; }
        public long FileSize { get; private set; }
        public string FileSizeStr { get; set; }
        public IReadOnlyDictionary<Object, IReadOnlyList<Object>> References { get; set; } 
        public bool IsReferences { get; set; }
        public IReadOnlyDictionary<int, DependencyUtil.SameAssetInfo> Compares { get; set; }
        public bool IsCompare { get; set; }
        public bool Changed { get; set; }

        public AssetInfo(string path, Texture2D tex, TextureImporter textureImporter)
        {
            Path = path;
            Texture2D = tex;
            TextureImporter = textureImporter;
            AOSSettings = TextureImporter.GetPlatformTextureSettings("Android");
            FormatType = AOSSettings.format;
            TextureType = TextureImporter.textureType;
            WrapMode = TextureImporter.wrapMode;
            FilterMode = TextureImporter.filterMode;
            MaxTextureSize = TextureImporter.maxTextureSize;
            FileSize = EditorTextureUtil.GetStorageMemorySize(Texture2D);
            FileSizeStr = EditorTextureUtil.TextureSize(Texture2D);
        }

        public void Reset()
        {
            if (!Changed)
            {
                return;
            }
            
            TextureType = TextureImporter.textureType;
            WrapMode = TextureImporter.wrapMode;
            FilterMode = TextureImporter.filterMode;
            MaxTextureSize = TextureImporter.maxTextureSize;
            ReSetTextureImporterFormat();
        }

        private void Refresh()
        {
            Texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(Path);
            FormatType = AOSSettings.format;
            FileSize = EditorTextureUtil.GetStorageMemorySize(Texture2D);
            FileSizeStr = EditorTextureUtil.TextureSize(Texture2D);
            Changed = false;
        }

        private void SetTextureImporter()
        {
            TextureImporter.textureType = TextureType;
            TextureImporter.wrapMode = WrapMode;
            TextureImporter.filterMode = FilterMode;
            TextureImporter.maxTextureSize = MaxTextureSize;
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

        public void ForceSetTextureImporterFormat(int formatIdx)
        {
            if (formatIdx < 0)
            {
                return;
            }
            
            var format = Enum.Parse<TextureImporterFormat>(TextureFormats[formatIdx]);
            AOSSettings.format = format;
            FormatType = format;
            Changed = true;
        }
        
        public void SetTextureImporterFormat(int formatIdx, bool showMsgBox)
        {
            if (formatIdx < 0)
            {
                return;
            }

            var format = Enum.Parse<TextureImporterFormat>(TextureFormats[formatIdx]);
            if (format == FormatType)
            {
                if (showMsgBox)
                {
                    EditorUtility.DisplayDialog("알림", "기존 포맷 타입과 동일합니다.", "확인");
                }
                return;
            }
            
            AOSSettings.format = format;
            FormatType = format;
            Changed = true;
        }
        
        public void ReSetTextureImporterFormat()
        {
            AOSSettings.format = TextureImporter.GetPlatformTextureSettings("Android").format;
            FormatType = AOSSettings.format;
            Changed = false;
        }
        
        public void Save()
        {
            SetTextureImporter();
            SetPlatformTextureSettings();
            Refresh();
        }
    }

    public int TotalCnt => _assetInfoMap.Sum(_ => _.Value.Count);
    public int SearchedCnt(string path) => _assetInfoMap[path].Count;
    public (AssetImporterConsts.SortTexture sortType, bool descending) CurSort { private get; set; }
    public AssetImporterConsts.FilterTexture CurFilterType { private get; set; }
    public IReadOnlyList<AssetInfo> SearchedAssetInfos => _searchedAssetInfos;
    private List<AssetInfo> _searchedAssetInfos = new();
    public IReadOnlyDictionary<string, IReadOnlyList<AssetInfo>> AssetInfoMap => _assetInfoMap;
    private readonly Dictionary<string, IReadOnlyList<AssetInfo>> _assetInfoMap = new();

    public void Initialize(IEnumerable<string> paths)
    {
        EditorUtility.DisplayProgressBar("텍스쳐를 불러오는 중입니다.", "", 1);
        CreateLabelsAndAssets(paths);
        EditorUtility.ClearProgressBar();
    }

    private void CreateLabelsAndAssets(IEnumerable<string> paths)
    {
        _assetInfoMap.Clear();
        
        foreach (var path in paths)
        {
            var guids = AssetDatabase.FindAssets("t:texture", new[] { path });

            CreateLabels(guids);
            _assetInfoMap.Add(path, CreateAssets(guids));
        }
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
    
    private IReadOnlyList<AssetInfo> CreateAssets(IReadOnlyList<string> guids)
    {
        var result = new List<AssetInfo>(guids.Count);

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
            {
                continue;
            }

            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter == null)
            {
                continue;
            }
            
            result.Add(new AssetInfo(path, tex, textureImporter));
        }

        return result;
    }
    
    public void CalcSearchedAssetInfos(
        string path,
        int selectedLabelIdx, 
        int selectedTextureMaxSizeIdx, 
        int selectedTextureMinSizeIdx,
        string searchedTextureName)
    {
        _searchedAssetInfos.Clear();

        var selectedAssetInfos = GetSelectedAssetInfos(path);
        foreach (var assetInfo in selectedAssetInfos)
        {
            if (TryCalcAssetInfo(selectedLabelIdx, selectedTextureMaxSizeIdx, selectedTextureMinSizeIdx, searchedTextureName, assetInfo))
            {
                _searchedAssetInfos.Add(assetInfo);
            }
        }

        Filter();
        Sort();
    }
    
    private IEnumerable<AssetInfo> GetSelectedAssetInfos(string path)
    {
        return path.IsNullOrEmpty() 
            ? _assetInfoMap.First().Value 
            : _assetInfoMap[path];
    }

    private bool TryCalcAssetInfo(
        int selectedLabelIdx, 
        int selectedTextureMaxSizeIdx, 
        int selectedTextureMinSizeIdx,
        string searchedTextureName,
        AssetInfo assetInfo)
    {
        var tex = assetInfo.Texture2D;
        if (!SearchedTextureName(tex.name))
        {
            return false;
        }
        
        var label = Labels[selectedLabelIdx];
        var textureMaxSize = int.Parse(TextureSizes[selectedTextureMaxSizeIdx]);
        var textureMinSize = int.Parse(TextureSizes[selectedTextureMinSizeIdx]);
        var existLabel = ExistLabel(tex, label);
        var checkSizeTexture = CheckSizeTexture(tex, textureMaxSize, textureMinSize);
        
        if (!label.Equals(_noneLabel) && textureMaxSize > 0)
        {
            return existLabel && checkSizeTexture;
        }

        if (textureMaxSize > 0)
        {
            if (checkSizeTexture)
            {
                return true;
            }
        }
        else
        {
            if (existLabel)
            {
                return true;
            }
        }
        return false;
        
        bool ExistLabel(Texture2D t, string l)
        {
            return AssetDatabase.GetLabels(t).Contains(l);
        }
        bool SearchedTextureName(string name)
        {
            return searchedTextureName.IsNullOrWhiteSpace() 
                || name.ToLower().Contains(searchedTextureName.ToLower());
        }
        bool CheckSizeTexture(Texture2D t, int maxSize, int minSize)
        {
            return t.width <= maxSize && t.height <= maxSize
                && t.width >= minSize && t.height >= minSize;
        }
    }
    
    private void Sort()
    {
        switch (CurSort.sortType)
        {
        case AssetImporterConsts.SortTexture.Name:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.Texture2D.name).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.Texture2D.name).ToList();
            }
            break;

        case AssetImporterConsts.SortTexture.FileSize:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.FileSize).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.FileSize).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.TextureSize:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.Texture2D.width).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.Texture2D.width).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.MipMap:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.TextureImporter.mipmapEnabled).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.TextureImporter.mipmapEnabled).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.Format:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.AOSSettings.format).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.AOSSettings.format).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.WrapMode:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.WrapMode).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.WrapMode).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.FilterMode:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.FilterMode).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.FilterMode).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.TextureType:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.TextureType).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.TextureType).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.References:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.IsReferences).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.IsReferences).ToList();
            }
            break;
        
        case AssetImporterConsts.SortTexture.Compare:
            {
                _searchedAssetInfos = CurSort.descending
                    ? _searchedAssetInfos.OrderByDescending(_ => _.IsCompare).ToList()
                    : _searchedAssetInfos.OrderBy(_ => _.IsCompare).ToList();
            }
            break;
        }
    }
    
    private void Filter()
    {
        switch (CurFilterType)
        {
        case AssetImporterConsts.FilterTexture.MipMap:
            {
                _searchedAssetInfos = _searchedAssetInfos.Where(_ => _.TextureImporter.mipmapEnabled).ToList();
            }
            break;

        case AssetImporterConsts.FilterTexture.References:
            {
                _searchedAssetInfos = _searchedAssetInfos.Where(_ => _.IsReferences).ToList();
            }
            break;

        case AssetImporterConsts.FilterTexture.Compare:
            {
                _searchedAssetInfos = _searchedAssetInfos.Where(_ => _.IsCompare).ToList();
            }
            break;
        }
    }
    
    public bool CanDiff()
    {
        return _assetInfoMap.SelectMany(pair => pair.Value).Any(assetInfo => assetInfo.Changed);
    }

    public bool TrySave()
    {
        var changed = false;
        Object activeObject = null;

        foreach (var pair in _assetInfoMap)
        {
            foreach (var assetInfo in pair.Value)
            {
                if (!assetInfo.Changed)
                {
                    continue;
                }

                activeObject = assetInfo.Texture2D;
                assetInfo.Save();
                changed = true;
            }
        }

        if (changed)
        {
            Selection.activeObject = activeObject;
        }
        
        return changed;
    }
}
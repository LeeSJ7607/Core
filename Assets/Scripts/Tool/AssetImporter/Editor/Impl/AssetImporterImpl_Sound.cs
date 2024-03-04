using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterImpl_Sound : IAssetImporterImpl
{
    public sealed class AssetInfo
    {
        public AudioClip AudioClip { get; }
        public AudioImporter AudioImporter { get; }
        public AudioImporterSampleSettings AOSSettings { get; }
        public bool ForceToMono { get; set; }
        public bool PreloadAudioData { get; set; }
        public AudioCompressionFormat CompressionFormat { get; set; }
        public AudioClipLoadType LoadType { get; set; }
        public long FileSize { get; } 
        public string FileSizeStr { get; }
        public IReadOnlyDictionary<Object, IReadOnlyList<Object>> References { get; set; } 
        public bool IsReferences { get; set; }
        public bool Changed { get; set; }
        
        public AssetInfo(AudioImporter importer)
        {
            AudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(importer.assetPath);
            AudioImporter = importer;
            AOSSettings = importer.GetOverrideSampleSettings("Android");
            ForceToMono = importer.forceToMono;
            PreloadAudioData = AOSSettings.preloadAudioData;
            CompressionFormat = AOSSettings.compressionFormat;
            LoadType = AOSSettings.loadType;
            FileSize = new FileInfo(importer.assetPath).Length;
            FileSizeStr = $"{FileSize / 1000:#,###} KB";
        }

        public void Reset()
        {
            if (!Changed)
            {
                return;
            }
            
            ForceToMono = AudioImporter.forceToMono;
            PreloadAudioData = AOSSettings.preloadAudioData;
            CompressionFormat = AOSSettings.compressionFormat;
            LoadType = AOSSettings.loadType;
            Changed = false;
        }

        public void Save()
        {
            AudioImporter.SaveAndReimport();
            Changed = false;
        }
    }
    
    public int TotalCnt => AssetInfoMap.Sum(_ => _.Value.Count);
    public int SearchedCnt(string path) => _assetInfoMap[path].Count;
    public (AssetImporterConsts.SortSound sortType, bool descending) CurSort { private get; set; }
    public IReadOnlyList<AssetInfo> SearchedAssetInfos => _searchedAssetInfos;
    private List<AssetInfo> _searchedAssetInfos = new();
    public IReadOnlyDictionary<string, List<AssetInfo>> AssetInfoMap => _assetInfoMap;
    private readonly Dictionary<string, List<AssetInfo>> _assetInfoMap = new();
    
    public void Initialize(IEnumerable<string> paths)
    {
        EditorUtility.DisplayProgressBar("사운드를 불러오는 중입니다.", "", 1);
        CreateAssets(paths);
        EditorUtility.ClearProgressBar();
    }
    
    private void CreateAssets(IEnumerable<string> paths)
    {
        _assetInfoMap.Clear();
        
        foreach (var path in paths)
        {
            var guids = AssetDatabase.FindAssets("t:AudioClip", new [] { path });
            if (guids.Empty())
            {
                continue;
            }

            if (!_assetInfoMap.ContainsKey(path))
            {
                _assetInfoMap.Add(path, new List<AssetInfo>(guids.Length));
            }

            var assets = _assetInfoMap[path];
            var canAddSearchedAssetInfos = _searchedAssetInfos.Empty();

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var importer = (AudioImporter)AssetImporter.GetAtPath(assetPath);
                if (importer == null)
                {
                    continue;
                }
                
                var assetInfo = new AssetInfo(importer);
                assets.Add(assetInfo);

                if (canAddSearchedAssetInfos)
                {
                    _searchedAssetInfos.Add(assetInfo);
                }
            }
        }
    }
    
    public void CalcSearchedAssetInfos(string path)
    {
        _searchedAssetInfos.Clear();
        _searchedAssetInfos.AddRange(_assetInfoMap[path]);

        Sort();
    }
    
    private void Sort()
    {
        switch (CurSort.sortType)
        {
        case AssetImporterConsts.SortSound.Name:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.AudioClip.name).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.AudioClip.name).ToList();
            }
            break;
        
        case AssetImporterConsts.SortSound.FileSize:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.FileSize).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.FileSize).ToList();
            }
            break;
        
        case AssetImporterConsts.SortSound.ForceToMono:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.ForceToMono).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.ForceToMono).ToList();
            }
            break;
        
        case AssetImporterConsts.SortSound.PreloadAudioData:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.PreloadAudioData).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.PreloadAudioData).ToList();
            }
            break;
        
        case AssetImporterConsts.SortSound.CompressionFormat:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.CompressionFormat).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.CompressionFormat).ToList();
            }
            break;
        
        case AssetImporterConsts.SortSound.LoadType:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.LoadType).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.LoadType).ToList();
            }
            break;
        
        case AssetImporterConsts.SortSound.References:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.IsReferences).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.IsReferences).ToList();
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

                activeObject = assetInfo.AudioClip;
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
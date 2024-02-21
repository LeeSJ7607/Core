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
        public bool ForceToMono { get; private set; }
        public bool PreloadAudioData { get; private set; }
        public AudioCompressionFormat CompressionFormat { get; private set; }
        public AudioClipLoadType LoadType { get; private set; }
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
    
    public bool CanDiff()
    {
        return _assetInfoMap.SelectMany(pair => pair.Value).Any(assetInfo => assetInfo.Changed);
    }
}
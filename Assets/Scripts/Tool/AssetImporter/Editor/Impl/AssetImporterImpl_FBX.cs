using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterImpl_FBX
{
    public sealed class AssetInfo
    {
        public AssetInfo(ModelImporter importer)
        {
            
        }
    }
    
    public IReadOnlyList<AssetInfo> SearchedAssetInfos => _searchedAssetInfos;
    private List<AssetInfo> _searchedAssetInfos = new();
    public IReadOnlyDictionary<string, List<AssetInfo>> AssetInfoMap => _assetInfoMap;
    private readonly Dictionary<string, List<AssetInfo>> _assetInfoMap = new();
    
    public void Initialize(IEnumerable<string> paths)
    {
        EditorUtility.DisplayProgressBar("FBX를 불러오는 중입니다.", "", 1);
        CreateAssets(paths);
        EditorUtility.ClearProgressBar();
    }

    private void CreateAssets(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            var guids = AssetDatabase.FindAssets("t:Model", new [] { path });
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
                var importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);
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
        return false;
    }
}
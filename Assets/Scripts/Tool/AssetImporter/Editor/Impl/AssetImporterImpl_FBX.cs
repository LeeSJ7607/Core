using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterImpl_FBX : IAssetImporterImpl
{
    public sealed class AssetInfo
    {
        public GameObject FBX { get; }
        public ModelImporter ModelImporter { get; }
        public ModelImporterNormals Normals { get; set; }
        public ModelImporterTangents Tangents { get; set; }
        public ModelImporterMeshCompression MeshCompression { get; set; }
        public bool IsReadable { get; set; }
        public long FileSize { get; } 
        public string FileSizeStr { get; }
        public IReadOnlyDictionary<Object, IReadOnlyList<Object>> References { get; set; } 
        public bool IsReferences { get; set; }
        public IReadOnlyDictionary<int, DependencyUtil.SameAssetInfo> Compares { get; set; }
        public bool IsCompare { get; set; }
        public bool Changed { get; set; }
        
        public AssetInfo(ModelImporter importer)
        {
            FBX = AssetDatabase.LoadAssetAtPath<GameObject>(importer.assetPath);
            ModelImporter = importer;
            Normals = importer.importNormals;
            Tangents = importer.importTangents;
            MeshCompression = importer.meshCompression;
            IsReadable = importer.isReadable;
            FileSize = new FileInfo(importer.assetPath).Length;
            FileSizeStr = $"{FileSize / 1000:#,###} KB";
        }
        
        public void Reset()
        {
            if (!Changed)
            {
                return;
            }
            
            Normals = ModelImporter.importNormals;
            Tangents = ModelImporter.importTangents;
            MeshCompression = ModelImporter.meshCompression;
            IsReadable = ModelImporter.isReadable;
            Changed = false;
        }
        
        private void Refresh()
        {
            Normals = ModelImporter.importNormals;
            Tangents = ModelImporter.importTangents;
            MeshCompression = ModelImporter.meshCompression;
            IsReadable = ModelImporter.isReadable;
            Changed = false;
        }
        
        private void SetModelImporter()
        {
            ModelImporter.importNormals = Normals;
            ModelImporter.importTangents = Tangents;
            ModelImporter.meshCompression = MeshCompression;
            ModelImporter.isReadable = IsReadable;
            ModelImporter.SaveAndReimport();
        }
        
        public void Save()
        {
            SetModelImporter();
            Refresh();
        }
    }
    
    public int TotalCnt => AssetInfoMap.Sum(_ => _.Value.Count);
    public int SearchedCnt(string path) => _assetInfoMap[path].Count;
    public (AssetImporterConsts.SortFBX sortType, bool descending) CurSort { private get; set; }
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
        _assetInfoMap.Clear();
        
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
        case AssetImporterConsts.SortFBX.Name:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.FBX.name).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.FBX.name).ToList();
            }
            break;

        case AssetImporterConsts.SortFBX.FileSize:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.FileSize).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.FileSize).ToList();
            }
            break;

        case AssetImporterConsts.SortFBX.ReadWrite:
            {
                _searchedAssetInfos = CurSort.descending 
                    ? _searchedAssetInfos.OrderByDescending(_ => _.ModelImporter.isReadable).ToList() 
                    : _searchedAssetInfos.OrderBy(_ => _.ModelImporter.isReadable).ToList();
            }
            break;
        
        case AssetImporterConsts.SortFBX.References:
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

                activeObject = assetInfo.FBX;
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
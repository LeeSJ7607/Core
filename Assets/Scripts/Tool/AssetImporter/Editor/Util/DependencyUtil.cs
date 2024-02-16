using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DependencyUtil
{
    public sealed class SameAssetInfo
    {
        public Texture Tex { get; }
        public Material Mat { get; }
        
        public SameAssetInfo(Texture tex, Material mat = null)
        {
            Tex = tex;
            Mat = mat;
        }
    }
    
    private static Dictionary<string, List<Object>> _dependencies;
    private static Dictionary<string, Dictionary<int, SameAssetInfo>> _sameAssets;
    
    private static void InitDependencies()
    {
        if (_dependencies != null)
        {
            return;
        }
        
        var assetPaths = AssetDatabase.GetAllAssetPaths().ToList();
        var totalCnt = assetPaths.Count;
        _dependencies = assetPaths.ToDictionary(_ => _, _ => new List<Object>());
        
        for (var i = 0; i < totalCnt; i++)
        {
            EditorUtility.DisplayProgressBar($"참조된 에셋들을 검색중입니다. ({i}/{totalCnt})", "", (float)i / totalCnt);
            
            var dependencies = AssetDatabase.GetDependencies(assetPaths[i], false);
            foreach (var dependency in dependencies)
            {
                if (_dependencies.ContainsKey(dependency) && dependency != assetPaths[i])
                {
                    _dependencies[dependency].Add(AssetDatabase.LoadAssetAtPath<Object>(assetPaths[i]));
                }
            }
        }
        
        EditorUtility.ClearProgressBar();
    }
    
    private static void InitSameAssets()
    {
        if (_sameAssets != null)
        {
            return;
        }
        
        var matPaths = AssetDatabase.FindAssets("t:material").Select(AssetDatabase.GUIDToAssetPath).ToArray();
        var texPaths = AssetDatabase.FindAssets("t:texture").Select(AssetDatabase.GUIDToAssetPath).ToArray();
        _sameAssets = texPaths.ToDictionary(_ => _, _ => new Dictionary<int, SameAssetInfo>());
        
        var totalCnt = _sameAssets.Count;
        var i = 0;
        
        foreach (var pair in _sameAssets)
        {
            EditorUtility.DisplayProgressBar($"동일한 텍스쳐를 검색중입니다. ({i}/{totalCnt})", "", (float)i / totalCnt);
            
            var targetPath = pair.Key;
            var targetTex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetPath);
            if (targetTex == null)
            {
                continue;
            }
            
            var sameAsset = _sameAssets[targetPath];
            
            foreach (var path in texPaths)
            {
                if (targetPath.Equals(path))
                {
                    continue;
                }

                var compareTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (compareTex == null)
                {
                    continue;
                }

                if (targetTex.imageContentsHash == compareTex.imageContentsHash)
                {
                    sameAsset.Add(compareTex.GetHashCode(), new SameAssetInfo(compareTex));
                }
            }
            
            foreach (var matPath in matPaths)
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                var textures = MaterialUtil.GetTextures(mat);
                
                foreach (var tex in textures)
                {
                    if (sameAsset.ContainsKey(tex.GetHashCode()))
                    {
                        sameAsset[tex.GetHashCode()] = new SameAssetInfo(tex, mat);
                        continue;
                    }
                    
                    if (targetTex.imageContentsHash == tex.imageContentsHash)
                    {
                        sameAsset.Add(tex.GetHashCode(), new SameAssetInfo(tex, mat));
                    }
                }
            }
            
            i++;
        }
        
        EditorUtility.ClearProgressBar();
    }
    
    private static (IReadOnlyDictionary<Object, IReadOnlyList<Object>> references, int cnt) GetDependencies(Object target)
    {
        var result = new Dictionary<Object, IReadOnlyList<Object>>();
        var targetPath = AssetDatabase.GetAssetPath(target);
        var dependency = _dependencies[targetPath];
        result.Add(target, dependency);
        
        return (result, dependency.Count);
    }
    
    public static void Dependencies(IEnumerable<AssetImporterImpl_FBX.AssetInfo> assetInfos)
    {
        InitDependencies();
        
        foreach (var assetInfo in assetInfos)
        {
            if (assetInfo.References != null)
            {
                continue;
            }

            var (references, cnt) = GetDependencies(assetInfo.FBX);
            assetInfo.References = references;
            assetInfo.IsReferences = cnt > 0;
        }
    }
    
    public static void Dependencies(IEnumerable<AssetImporterImpl_Texture.AssetInfo> assetInfos)
    {
        InitDependencies();
        
        foreach (var assetInfo in assetInfos)
        {
            if (assetInfo.References != null)
            {
                continue;
            }

            var (references, cnt) = GetDependencies(assetInfo.Texture2D);
            assetInfo.References = references;
            assetInfo.IsReferences = cnt > 0;
        }
    }
    
    public static void SameAssets(IEnumerable<AssetImporterImpl_Texture.AssetInfo> assetInfos)
    {
        InitSameAssets();
        
        foreach (var assetInfo in assetInfos)
        {
            if (assetInfo.Compares != null)
            {
                continue;
            }

            var sameAsset = _sameAssets[assetInfo.TextureImporter.assetPath];
            if (sameAsset.ContainsKey(assetInfo.Texture2D.GetHashCode()))
            {
                sameAsset.Remove(assetInfo.Texture2D.GetHashCode());
            }
            
            assetInfo.Compares = sameAsset;
            assetInfo.IsCompare = sameAsset.Count > 0;
        }
    }
}
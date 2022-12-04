using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DependencyImpl
{
    private static Dictionary<string, List<string>> _dependencies;
    private static Dictionary<string, List<Texture>> _sameAssets;
    
    private static void InitDependencies()
    {
        if (_dependencies != null)
        {
            return;
        }
        
        var assetPaths = AssetDatabase.GetAllAssetPaths().ToList();
        var totalCnt = assetPaths.Count;
        _dependencies = assetPaths.ToDictionary(_ => _, _ => new List<string>());
        
        for (var i = 0; i < totalCnt; i++)
        {
            EditorUtility.DisplayProgressBar("참조된 에셋들을 검색중입니다.", "", (float)i / totalCnt);
            
            var dependencies = AssetDatabase.GetDependencies(assetPaths[i], false);
            foreach (var dependency in dependencies)
            {
                if (_dependencies.ContainsKey(dependency) && dependency != assetPaths[i])
                {
                    _dependencies[dependency].Add(assetPaths[i]);
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
        _sameAssets = texPaths.ToDictionary(_ => _, _ => new List<Texture>());

        foreach (var pair in _sameAssets)
        {
            var targetPath = pair.Key;
            var targetTex = AssetDatabase.LoadAssetAtPath<Texture2D>(targetPath);

            for (var i = 0; i < texPaths.Length; i++)
            {
                EditorUtility.DisplayProgressBar("텍스쳐를 검색중입니다.", "", (float)i / texPaths.Length);
                
                var path = texPaths[i];
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                if (targetPath.Equals(path) == false 
                 && targetTex.imageContentsHash == tex.imageContentsHash)
                {
                    _sameAssets[targetPath].Add(tex);
                }
            }

            for (var i = 0; i < matPaths.Length; i++)
            {
                EditorUtility.DisplayProgressBar("메테리얼을 검색중입니다.", "", (float)i / matPaths.Length);
                
                var path = matPaths[i];
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                var tex = mat.mainTexture;
                if (tex == null)
                {
                    continue;
                }
                
                if (targetPath.Equals(AssetDatabase.GetAssetPath(tex)) == false 
                 && targetTex.imageContentsHash == tex.imageContentsHash)
                {
                    _sameAssets[targetPath].Add(tex);
                }
            }
        }
        
        EditorUtility.ClearProgressBar();
    }
    
    private static IReadOnlyDictionary<Object, IReadOnlyList<string>> GetDependencies(Object target)
    {
        var result = new Dictionary<Object, IReadOnlyList<string>>();
        var targetPath = AssetDatabase.GetAssetPath(target);

        if (_dependencies.ContainsKey(targetPath))
        {
            result.Add(target, _dependencies[targetPath]);
        }

        return result;
    }

    public static void Dependencies(IEnumerable<AssetImporter_TextureImpl.AssetInfo> assetInfos)
    {
        InitDependencies();
        
        foreach (var assetInfo in assetInfos)
        {
            if (assetInfo.References != null)
            {
                continue;
            }
            
            var dependencies = GetDependencies(assetInfo.Texture2D);
            assetInfo.References = dependencies;
            assetInfo.IsReferences = dependencies[assetInfo.Texture2D].Count > 0;
        }
    }
    
    public static void SameAssets(IEnumerable<AssetImporter_TextureImpl.AssetInfo> assetInfos)
    {
        InitSameAssets();

        foreach (var assetInfo in assetInfos)
        {
            assetInfo.IsCompare = _sameAssets[assetInfo.TextureImporter.assetPath].Count > 0;
        }
    }
}
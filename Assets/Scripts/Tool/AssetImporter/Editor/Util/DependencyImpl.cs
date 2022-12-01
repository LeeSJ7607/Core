using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DependencyImpl
{
    private static readonly Dictionary<string, List<string>> _dependencies;

    static DependencyImpl()
    {
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

    public static IReadOnlyDictionary<Object, IReadOnlyList<string>> GetDependencies(Object target)
    {
        var result = new Dictionary<Object, IReadOnlyList<string>>();
        var targetPath = AssetDatabase.GetAssetPath(target);

        if (_dependencies.ContainsKey(targetPath))
        {
            result.Add(target, _dependencies[targetPath]);
        }

        return result;
    }

    public static void AllAssetCalcReferences(IEnumerable<AssetImporter_TextureImpl.AssetInfo> assetInfos)
    {
        foreach (var searchedAssetInfo in assetInfos)
        {
            searchedAssetInfo.CalcReferences();
        }
    }
}
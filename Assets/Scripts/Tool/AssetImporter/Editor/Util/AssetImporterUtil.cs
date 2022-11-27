using System.Linq;
using System.Collections.Generic;
using UnityEditor;

public static class AssetImporterUtil
{
    public static IReadOnlyDictionary<UnityEngine.Object, IReadOnlyList<string>> CalcReferences(UnityEngine.Object target)
    {
        return GetDependencies(target, CalcDependencies());
    }

    private static IReadOnlyDictionary<string, List<string>> CalcDependencies()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths().ToList();
        var totalCnt = assetPaths.Count;
        var dic = assetPaths.ToDictionary(_ => _, _ => new List<string>());

        for (var i = 0; i < totalCnt; i++)
        {
            EditorUtility.DisplayProgressBar("찾는 중", "", (float)i / totalCnt);
            
            var dependencies = AssetDatabase.GetDependencies(assetPaths[i], false);
            foreach (var dependency in dependencies)
            {
                if (dic.ContainsKey(dependency) && dependency != assetPaths[i])
                {
                    dic[dependency].Add(assetPaths[i]);
                }
            }
        }
        
        EditorUtility.ClearProgressBar();
        return dic;
    }

    private static IReadOnlyDictionary<UnityEngine.Object, IReadOnlyList<string>> GetDependencies(
        UnityEngine.Object target,
        IReadOnlyDictionary<string, List<string>> map)
    {
        var result = new Dictionary<UnityEngine.Object, IReadOnlyList<string>>();
        var targetPath = AssetDatabase.GetAssetPath(target);

        if (map.ContainsKey(targetPath))
        {
            result.Add(target, map[targetPath]);
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
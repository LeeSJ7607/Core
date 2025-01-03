using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

internal sealed class TableWindowImpl
{
    public void Initialize(string excelFolderPath)
    {
        var types = typeof(iBaseTable).Assembly
                                      .GetExportedTypes()
                                      .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                                      .Where(_ => typeof(iBaseTable).IsAssignableFrom(_))
                                      .ToArray();

        foreach (var type in types)
        {
            Debug.Log(type.Name);
        }
        
        foreach (var type in types)
        {
            var csvPath = $"{excelFolderPath}/{type.Name}.csv";

            if (File.Exists(csvPath) == false)
            {
                
            }
            
            var lines = File.ReadAllLines(csvPath, Encoding.UTF8);

            const int valueStartIdx = 1;
            var csv = GetCSV(lines);
            var result = new List<Dictionary<string, string>>();
            for (var row = valueStartIdx; row < lines.Length; row++)
            {
                result.Add(SetPropertyValue(row, csv));
            }

            if (TryGetTableAsset(type, out var tableAsset) == false)
            {
                continue;
            }

            if (tableAsset.TryParse(result))
            {
                EditorUtility.SetDirty(tableAsset as ScriptableObject);
            }
        }
        
        (string[] properties, List<string[]> values) GetCSV(IReadOnlyList<string> lines_)
        {
            return (lines_[0].Split(','), lines_.Select(line => line.Split(',')).ToList());
        }

        Dictionary<string, string> SetPropertyValue(int row_, (string[] properties_, List<string[]> values_) csv_)
        {
            var (properties, values) = csv_;
            var dic = new Dictionary<string, string>();
            
            for (var i = 0; i < properties.Length; i++)
            {
                dic.Add(properties[i], values[row_][i]);
            }

            return dic;
        }
        
        
    }
    
    private static bool TryGetTableAsset(Type type_, out iBaseTable baseTable_)
    {
        var instance = ScriptableObject.CreateInstance(type_);
        if (instance is not iBaseTable table)
        {

            baseTable_ = null;
            return false;
        }

        var fullPath = $"Assets";
        if (Directory.Exists(fullPath) == false)
        {
            Directory.CreateDirectory(fullPath);
        }

        var assetPath = $"{fullPath}/{type_.Name}.asset";
        if (File.Exists(assetPath) == false)
        {
            AssetDatabase.CreateAsset(instance, assetPath);
        }

        baseTable_ = AssetDatabase.LoadAssetAtPath(assetPath, type_) as iBaseTable;
        return true;
    }

    public void Parse()
    {
     
    }
}
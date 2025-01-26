using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal sealed class TableWindowLogic
{
    internal sealed class TableInfo
    {
        public Type TableType { get; }
        public string TableName => TableType.Name;
        private string KeyBakeTime => $"{nameof(TableInfo)}_{TableName}";
        public string BakeTimeStr { get; private set; }
        public bool HasTableFile { get; set; }

        public TableInfo(Type type)
        {
            TableType = type;
            BakeTimeStr = EditorPrefs.GetString(KeyBakeTime, null);
        }

        public void SetBakeTime()
        {
            BakeTimeStr = DateTime.Now.ToString();
            EditorPrefs.SetString(KeyBakeTime, BakeTimeStr);
        }
    }
    
    public TableInfo[] TableInfos { get; private set; }
    private string _selectedExcelFolderPath;
    private string _selectedOutputFolderPath;
    
    public bool Initialize(string selectedExcelFolderPath, string selectedOutputFolderPath)
    {
        _selectedOutputFolderPath = selectedOutputFolderPath;
        return TrySetExcelFolderPath(selectedExcelFolderPath) && CreateTables();
    }

    private bool TrySetExcelFolderPath(string excelFolderPath)
    {
        if (excelFolderPath.IsNullOrEmpty())
        {
            return false;
        }
        
        if (!DirectoryUtil.HasFile(excelFolderPath, $"*{TableWindow.TABLE_EXTENSION}"))
        {
            Debug.LogError("No Table files found in the directory.");
            return false;
        }
        
        _selectedExcelFolderPath = excelFolderPath;
        return true;
    }

    private bool CreateTables()
    {
        var types = typeof(IBaseTable).Assembly
                                     .GetExportedTypes()
                                     .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                                     .Where(_ => typeof(IBaseTable).IsAssignableFrom(_))
                                     .ToArray();

        if (types.IsNullOrEmpty())
        {
            Debug.LogError("No table classes defined.");
            return false;
        }

        TableInfos = new TableInfo[types.Length];
        for (var i = 0; i < types.Length; i++)
        {
            TableInfos[i] = new TableInfo(types[i]);
            var tableInfo = TableInfos[i];
            
            var tablePath = $"{_selectedExcelFolderPath}/{tableInfo.TableName}{TableWindow.TABLE_EXTENSION}";
            tableInfo.HasTableFile = File.Exists(tablePath);
        }

        return true;
    }
    
    public void BakeTable((bool toggle, TableInfo tableInfo)[] checkToggleTables)
    {
        if (checkToggleTables.Length != TableInfos.Length)
        {
            Debug.LogError("The number of tables and the number of checkToggleTables are different.");
            return;
        }

        foreach (var (toggle, tableInfo) in checkToggleTables)
        {
            if (!toggle)
            {
                continue;
            }

            var tablePath = $"{_selectedExcelFolderPath}/{tableInfo.TableName}{TableWindow.TABLE_EXTENSION}";
            if (!File.Exists(tablePath))
            {
                Debug.LogError("Table file not found.");
                continue;
            }
        
            var tableFileLines = File.ReadAllLines(tablePath, Encoding.UTF8);
            if (tableFileLines.IsNullOrEmpty())
            {
                Debug.LogError("Table file is empty.");
                continue;
            }

            if (tableFileLines[1].IsNullOrEmpty())
            {
                Debug.LogError("Table file first row is empty.");
                continue;
            }

            var rows = ConvertTable(tableFileLines);
            var createTable = GetOrCreateTable(tableInfo.TableType);
            if (!createTable.TryParse(rows))
            {
                continue;
            }
            
            tableInfo.SetBakeTime();
            EditorUtility.SetDirty(createTable as ScriptableObject);
        }
    }

    private IReadOnlyList<Dictionary<string, string>> ConvertTable(string[] tableFileLines)
    {
        var rows = new List<Dictionary<string, string>>();
        var columns = tableFileLines[0].Split(',');
        var rawRows = tableFileLines.Skip(1).Select(row => row.Split(',')).ToArray();

        foreach (var rawRow in rawRows)
        {
            var rowData = new Dictionary<string, string>();
            for (var i = 0; i < columns.Length; i++)
            {
                rowData.Add(columns[i], rawRow[i]);
            }
            
            rows.Add(rowData);
        }

        return rows;
    }
    
    private IBaseTable GetOrCreateTable(Type type)
    {
        var instance = ScriptableObject.CreateInstance(type);
        var assetPath = $"{_selectedOutputFolderPath}/{type.Name}.asset";
        
        if (!File.Exists(assetPath))
        {
            AssetDatabase.CreateAsset(instance, assetPath);
        }

        return (IBaseTable)AssetDatabase.LoadAssetAtPath(assetPath, type);
    }
}
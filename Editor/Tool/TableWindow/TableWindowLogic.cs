using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

internal sealed class TableWindowLogic
{
    internal sealed class TableInfo
    {
        private Type _tableType;
        public Type TableType => GetTableType();
        public string TableName { get; }
        public string BakeTimeStr { get; private set; }
        public bool HasTableFile { get; }
        private string KeyBakeTime => $"{nameof(TableInfo)}_{TableName}";

        public TableInfo(string tableFile, string selectedTableFolderPath)
        {
            TableName = Path.GetFileNameWithoutExtension(tableFile);;
            BakeTimeStr = EditorPrefs.GetString(KeyBakeTime, null);
            
            var tablePath = $"{selectedTableFolderPath}/{TableName}{TableWindow.TABLE_EXTENSION}";
            HasTableFile = File.Exists(tablePath);
        }

        public void SetBakeTime()
        {
            BakeTimeStr = DateTime.Now.ToString();
            EditorPrefs.SetString(KeyBakeTime, BakeTimeStr);
        }

        private Type GetTableType()
        {
            return _tableType ?? (_tableType = typeof(IBaseTable).Assembly
                                                                 .GetExportedTypes()
                                                                 .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                                                                 .Where(_ => typeof(IBaseTable).IsAssignableFrom(_))
                                                                 .FirstOrDefault(_ => _.Name.Equals(TableName)));
        }
    }
    
    public TableInfo[] TableInfos { get; private set; }
    private string _selectedTableFolderPath;
    private string _selectedSOCreationPath;
    
    public bool Initialize(string selectedTableFolderPath, string selectedSOCreationPath)
    {
        _selectedSOCreationPath = selectedSOCreationPath;
        return TrySetTableFolderPath(selectedTableFolderPath) && CreateTables();
    }

    private bool TrySetTableFolderPath(string tableFolderPath)
    {
        if (tableFolderPath.IsNullOrEmpty())
        {
            return false;
        }
        
        if (!DirectoryUtil.HasFile(tableFolderPath, $"*{TableWindow.TABLE_EXTENSION}"))
        {
            Debug.LogError("No Table files found in the directory.");
            return false;
        }
        
        _selectedTableFolderPath = tableFolderPath;
        return true;
    }

    private bool CreateTables()
    {
        var tableFiles = Directory.GetFiles(_selectedTableFolderPath);
        TableInfos = new TableInfo[tableFiles.Length];
        
        for (var i = 0; i < tableFiles.Length; i++)
        {
            TableInfos[i] = new TableInfo(tableFiles[i], _selectedTableFolderPath);
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

            var tablePath = $"{_selectedTableFolderPath}/{tableInfo.TableName}{TableWindow.TABLE_EXTENSION}";
            if (!File.Exists(tablePath))
            {
                Debug.LogError("Table file not found.");
                continue;
            }
        
            var tableLines = File.ReadAllLines(tablePath, Encoding.UTF8);
            if (tableLines.IsNullOrEmpty())
            {
                Debug.LogError("Table file is empty.");
                continue;
            }

            if (tableLines[1].IsNullOrEmpty())
            {
                Debug.LogError("Table file first row is empty.");
                continue;
            }

            var rows = ConvertTable(tableLines);
            if (!TryGetOrCreateTable(tableInfo.TableType, out var createTable))
            {
                Debug.LogError($"{tableInfo.TableName}.cs is not found.");
                continue;
            }
            
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
                var column = columns[i];
                var row = rawRow[i];
                rowData.Add(column, row);
            }
            
            rows.Add(rowData);
        }

        return rows;
    }
    
    private bool TryGetOrCreateTable(Type tableType, out IBaseTable refBaseTable)
    {
        if (tableType == null)
        {
            refBaseTable = null;
            return false;
        }
        
        var assetPath = $"{_selectedSOCreationPath}/{tableType.Name}.asset";
        
        if (!File.Exists(assetPath))
        {
            var instance = ScriptableObject.CreateInstance(tableType);
            AssetDatabase.CreateAsset(instance, assetPath);
        }

        AddAddressTable(assetPath);
        refBaseTable = (IBaseTable)AssetDatabase.LoadAssetAtPath(assetPath, tableType);
        return true;
    }

    private void AddAddressTable(string assetPath)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = GetOrCreateGroup();
        var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group, false, false);
        var fileName = Path.GetFileNameWithoutExtension(assetPath);
        
        entry.SetAddress(fileName);
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
    }

    private AddressableAssetGroup GetOrCreateGroup()
    {
        const string groupName = "Table";
        var groupPath = $"Assets/AddressableAssetsData/AssetGroups/{groupName}.asset";
        
        if (File.Exists(groupPath))
        {
            return AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(groupPath);
        }

        var group = ScriptableObject.CreateInstance<AddressableAssetGroup>();
        group.Name = groupName;
        group.AddSchema<BundledAssetGroupSchema>();
        group.AddSchema<ContentUpdateGroupSchema>();
        AssetDatabase.CreateAsset(group, groupPath);
        
        return group;
    }
}
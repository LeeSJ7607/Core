using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

internal sealed class TableWindowLogic
{
    internal sealed class TableInfo
    {
        private readonly Type _tableType;
        public string TableName => _tableType.Name;
        public string BakeTimeStr { get; private set; }
        public bool HasTableFile { get; set; }

        public TableInfo(Type type)
        {
            _tableType = type;
            BakeTimeStr = PlayerPrefs.GetString(KeyBakeTime, null);
        }

        public void SetBakeTime()
        {
            BakeTimeStr = DateTime.Now.ToString();
            PlayerPrefs.SetString(KeyBakeTime, BakeTimeStr);
        }
        
        private string KeyBakeTime => $"{nameof(TableInfo)}_{TableName}";
    }
    
    public TableInfo[] TableInfos { get; private set; }
    private string _selectedExcelFolderPath;
    
    public bool Initialize(string selectedExcelFolderPath)
    {
        if (!TrySetExcelFolderPath(selectedExcelFolderPath))
        {
            return false;
        }
        
        return CreateTables();
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
        var types = typeof(BaseTable).Assembly
                                     .GetExportedTypes()
                                     .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                                     .Where(_ => typeof(BaseTable).IsAssignableFrom(_))
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
        
            var fileLines = File.ReadAllLines(tablePath, Encoding.UTF8);
            if (fileLines.IsNullOrEmpty())
            {
                continue;
            }
            
            var columns = fileLines[0].Split(',');
            var rows = fileLines.Skip(1).Select(row => row.Split(',')).ToArray();
            
            tableInfo.SetBakeTime();
        }
    }
    
    private bool TryGetTableAsset(out BaseTable tableAsset)
    {
        var instance = ScriptableObject.CreateInstance<BaseTable>();
        tableAsset = null;
        return true;
    }
}
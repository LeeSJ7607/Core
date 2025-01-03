using System;
using System.Linq;
using UnityEngine;

internal sealed class TableWindowLogic
{
    internal sealed class TableInfo
    {
        public Type TableType { get; }
        public string BakeTimeStr { get; private set; }

        public TableInfo(Type type)
        {
            TableType = type;
            BakeTimeStr = PlayerPrefs.GetString(KeyBakeTime, null);
        }

        public void SetBake()
        {
            BakeTimeStr = DateTime.Now.ToString();
            PlayerPrefs.SetString(KeyBakeTime, BakeTimeStr);
        }
        
        private string KeyBakeTime => $"{nameof(TableInfo)}_{TableType.Name}";
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
            
            //TODO: 베이크 시작.
            tableInfo.SetBake();
        }
    }
}
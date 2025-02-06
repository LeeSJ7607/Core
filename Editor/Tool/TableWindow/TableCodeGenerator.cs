using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal sealed class TableCodeGenerator
{
    public void Execute(string tableFolderPath, string scriptCreationPath, (bool toggle, TableWindowLogic.TableInfo tableInfo)[] checkToggleTables)
    {
        var sb = new StringBuilder(128);
        var selectedTableNames = CalcSelectedTableNames(checkToggleTables);
        var tableFiles = Directory.GetFiles(tableFolderPath);

        foreach (var tableFile in tableFiles)
        {
            if (!CanCodeGenerator(tableFile, selectedTableNames))
            {
                continue;
            }
            
            var readAllTextTableTemplate = ReadAllTextTableTemplate();
            sb.Append(readAllTextTableTemplate);
            
            var className = Path.GetFileNameWithoutExtension(tableFile);
            sb.Replace("#CLASSNAME#", className);

            if (!TryReadAllLinesTable(tableFile, out var refTableLines))
            {
                continue;
            }
            
            var field = WriteField(refTableLines);
            sb.Replace("#FIELD#", field);
            
            var path = Path.Combine(scriptCreationPath, $"{className}.cs");
            File.Delete(path);
            File.WriteAllText(path, sb.ToString());
            
            sb.Clear();
        }
        
        AssetDatabase.Refresh();
    }

    private static IReadOnlyList<string> CalcSelectedTableNames((bool toggle, TableWindowLogic.TableInfo tableInfo)[] checkToggleTables)
    {
        var selectedTableNames = new List<string>();
        
        foreach (var (toggle, tableInfo) in checkToggleTables)
        {
            if (!toggle)
            {
                continue;
            }
            
            selectedTableNames.Add(tableInfo.TableName);
        }

        return selectedTableNames;
    }

    private static bool CanCodeGenerator(string tableFile, IReadOnlyList<string> selectedTableNames)
    {
        foreach (var tableName in selectedTableNames)
        {
            if (tableFile.Contains(tableName))
            {
                return true;
            }
        }
        
        return false;
    }

    private static string ReadAllTextTableTemplate()
    {
        var guids = AssetDatabase.FindAssets("t:TextAsset TableTemplate");

        if (guids.Length == 0)
        {
            Debug.LogError("Not Found TableTemplate.txt");
            return string.Empty;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        return File.ReadAllText(assetPath);
    }

    private static bool TryReadAllLinesTable(string tablePath, out string[] refTableLines)
    {
        var tableLines = File.ReadAllLines(tablePath, Encoding.UTF8);
        if (tableLines.IsNullOrEmpty())
        {
            Debug.LogError("Table file is empty.");

            refTableLines = null;
            return false;
        }

        if (tableLines[1].IsNullOrEmpty())
        {
            Debug.LogError("Table file first row is empty.");
            
            refTableLines = null;
            return false;
        }

        refTableLines = tableLines;
        return true;
    }

    private static string WriteField(string[] tableLines)
    {
        const int rowStartIdx = 1;
        
        var field = string.Empty;
        var columns = tableLines[0].Split(',');
        var rows = tableLines[rowStartIdx].Split(',');

        for (var i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
            if (!CanWriteField(column))
            {
                continue;
            }
            
            if (i > 0)
            {
                field += "\n\t\t";
            }

            var searchRowIndex = rowStartIdx + 1;
            var row = FindValidRowValue(rows, i, tableLines, searchRowIndex);
            
            if (int.TryParse(row, out _))
            {
                field += $"public int {column};";
                continue;
            }
            
            if (long.TryParse(row, out _))
            {
                field += $"public long {column};";
                continue;
            }
            
            if (float.TryParse(row, out _))
            {
                field += $"public float {column};";
                continue;
            }
            
            if (bool.TryParse(row, out _))
            {
                field += $"public bool {column};";
                continue;
            }
            
            field += $"public string {column};";
        }

        return field;
    }

    private static string FindValidRowValue(string[] rows, int rowIdx, string[] tableLines, int tableLineIdx)
    {
        var row = rows[rowIdx];
        if (!row.IsNullOrWhiteSpace())
        {
            return row;
        }
        
        if (tableLineIdx >= tableLines.Length)
        {
            return string.Empty;
        }
        
        var tempRows = tableLines[tableLineIdx].Split(',');
        if (tempRows.IsNullOrEmpty())
        {
            return string.Empty;
        }
        
        var tempRow = tempRows[rowIdx];
        if (!tempRow.IsNullOrWhiteSpace())
        {
            return tempRow;
        }
        
        return FindValidRowValue(rows, rowIdx, tableLines, tableLineIdx + 1);
    }

    private static bool CanWriteField(string column)
    {
        var columnLower = column.ToLower();
        var filters = new[] { "memo" };

        foreach (var filter in filters)
        {
            if (columnLower.Contains(filter))
            {
                return false;
            }
        }
        
        return true;
    }
}
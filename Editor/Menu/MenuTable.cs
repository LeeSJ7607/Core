using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

internal sealed class MenuTable
{
    [MenuItem("Custom/Menu/CodeGenTable")]
    private static void CodeGenTable()
    {
        var sb = new StringBuilder(128);
        var excelFiles = Directory.GetFiles(TableWindow.SelectedExcelFolderPath);

        foreach (var excelFile in excelFiles)
        {
            sb.Append(ReadAllTextTableTemplate());
            
            var className = Path.GetFileNameWithoutExtension(excelFile);
            sb.Replace("#CLASSNAME#", className);

            if (!TryReadAllLineExcel(excelFile, out var refReadExcel))
            {
                continue;
            }
            
            
        }
    }

    private static string ReadAllTextTableTemplate()
    {
        var guids = AssetDatabase.FindAssets("TableTemplate.txt");

        if (guids.Length == 0)
        {
            Debug.LogError("Not Found TableTemplate.txt");
            return string.Empty;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        return File.ReadAllText(assetPath);
    }

    private static bool TryReadAllLineExcel(string excelPath, out string[] refReadExcel)
    {
        var tableFileLines = File.ReadAllLines(excelPath, Encoding.UTF8);

        if (tableFileLines.IsNullOrEmpty())
        {
            Debug.LogError("Table file is empty.");

            refReadExcel = null;
            return false;
        }

        if (tableFileLines[1].IsNullOrEmpty())
        {
            Debug.LogError("Table file first row is empty.");
            
            refReadExcel = null;
            return false;
        }

        refReadExcel = tableFileLines;
        return true;
    }
    
    private static void AddFieldTemplate(string columns)
    {
        for (var i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
        }
    }
}
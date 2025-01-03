using UnityEditor;
using UnityEngine;

internal sealed class TableWindow : EditorWindow
{
    private static readonly string KEY_EXCEL_FOLDER_PATH = $"{typeof(TableWindow)}_{KEY_EXCEL_FOLDER_PATH}";
    private string _excelFolderPath;
    private readonly TableWindowImpl _tableWindowImpl = new();
    
    [MenuItem("Custom/TableWindow")]
    public static void Open()
    {
        var tool = GetWindow<TableWindow>();
        var excelFolderPath = tool._excelFolderPath = PlayerPrefs.GetString(KEY_EXCEL_FOLDER_PATH);

        if (excelFolderPath.IsNullOrEmpty())
        {
            return;
        }
        
        tool._tableWindowImpl.Initialize(excelFolderPath);
    }

    private void OnGUI()
    {
        DrawFolderPath();
        DrawParseAll();
        DrawParse();
    }

    private void DrawFolderPath()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button("Specify the folder path", GUILayout.Width(140)))
        {
            _excelFolderPath = EditorUtility.OpenFolderPanel("Specify the folder path", _excelFolderPath, "");
            if (_excelFolderPath.IsNullOrEmpty())
            {
                return;
            }
            
            _tableWindowImpl.Initialize(_excelFolderPath);
            PlayerPrefs.SetString(KEY_EXCEL_FOLDER_PATH, _excelFolderPath);
        }
        
        GUILayout.Label(_excelFolderPath);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawParseAll()
    {
        if (GUILayout.Button("Test"))
        {
            _tableWindowImpl.Parse();
        }
    }
    
    private void DrawParse()
    {
        
    }
}
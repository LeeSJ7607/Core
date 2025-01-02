using UnityEditor;
using UnityEngine;

internal sealed class TableWindow : EditorWindow
{
    private static readonly string KEY_SELECTED_FOLDER_PATH = $"{typeof(TableWindow)}_{KEY_SELECTED_FOLDER_PATH}";
    private string _selectedFolderPath;
    
    [MenuItem("Custom/TableWindow")]
    public static void Open()
    {
        GetWindow<TableWindow>();
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
            _selectedFolderPath = EditorUtility.OpenFolderPanel("Specify the folder path", _selectedFolderPath, "");
            PlayerPrefs.SetString(KEY_SELECTED_FOLDER_PATH, _selectedFolderPath);
        }
        
        GUILayout.Label(_selectedFolderPath);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawParseAll()
    {
        
    }
    
    private void DrawParse()
    {
        
    }
}
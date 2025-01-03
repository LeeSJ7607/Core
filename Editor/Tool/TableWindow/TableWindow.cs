using UnityEditor;
using UnityEngine;

internal sealed class TableWindow : EditorWindow
{
    public const string TABLE_EXTENSION = ".csv";
    private static readonly string KEY_EXCEL_FOLDER_PATH = $"{typeof(TableWindow)}_{KEY_EXCEL_FOLDER_PATH}";
    private readonly TableWindowLogic _tableWindowLogic = new();
    private (bool toggle, TableWindowLogic.TableInfo tableInfo)[] _checkToggleTables;
    private string _selectedExcelFolderPath;
    private string _searchedTableName;
    private Vector2 _tableListScrollPos;
    
    [MenuItem("Custom/TableWindow")]
    public static void Open()
    {
        var tool = GetWindow<TableWindow>();
        var excelFolderPath = tool._selectedExcelFolderPath = PlayerPrefs.GetString(KEY_EXCEL_FOLDER_PATH);

        if (excelFolderPath.IsNullOrEmpty())
        {
            return;
        }
        
        tool.CreateTableWindowLogic();
    }
    
    private void CreateTableWindowLogic()
    {
        if (!_tableWindowLogic.Initialize(_selectedExcelFolderPath))
        {
            _checkToggleTables = null;
            return;
        }

        _checkToggleTables = new (bool, TableWindowLogic.TableInfo)[_tableWindowLogic.TableInfos.Length];
            
        for (var i = 0; i < _checkToggleTables.Length; i++)
        {
            _checkToggleTables[i].toggle = false;
            _checkToggleTables[i].tableInfo = _tableWindowLogic.TableInfos[i];
        }
    }

    private void OnGUI()
    {
        DrawTableFolderPath();
        
        if (_checkToggleTables.IsNullOrEmpty())
        {
            return;
        }
        
        EditorGUILayout.BeginHorizontal();
        DrawTableList();
        DrawSelectedTableInfo();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSelectedTableInfo()
    {
        EditorGUILayout.BeginVertical();
        
        foreach (var (toggle, tableInfo) in _checkToggleTables)
        {
            if (!toggle)
            {
                continue;
            }
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUIUtil.Desc($"{tableInfo.TableName}{TABLE_EXTENSION}");

            if (tableInfo.HasTableFile)
            {
                GUIUtil.Desc($"Bake Time: {tableInfo.BakeTimeStr}");
            }
            else
            {
                GUIUtil.Desc("Table file not found.", Color.red);
            }
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawTableFolderPath()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUIUtil.Btn("Specify the folder path", 140, () =>
        {
            var excelFolderPath = EditorUtility.OpenFolderPanel("Specify the folder path", _selectedExcelFolderPath, "");
            if (excelFolderPath.IsNullOrEmpty())
            {
                return;
            }

            _selectedExcelFolderPath = excelFolderPath;
            CreateTableWindowLogic();
            PlayerPrefs.SetString(KEY_EXCEL_FOLDER_PATH, _selectedExcelFolderPath);
        });
        
        GUILayout.Label(_selectedExcelFolderPath);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTableList()
    {
        if (_tableWindowLogic.TableInfos.IsNullOrEmpty())
        {
            return;
        }
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(300));
        _searchedTableName = GUILayout.TextField(_searchedTableName);
        DrawToggleTables();
        DrawBakeSelectedAndBakeAll();
        DrawSelectAndDeSelectBtn();
        EditorGUILayout.EndVertical();
    }

    private void DrawToggleTables()
    {
        _tableListScrollPos = EditorGUILayout.BeginScrollView(_tableListScrollPos);
        
        for (var i = 0; i < _tableWindowLogic.TableInfos.Length; i++)
        {
            var tableInfo = _tableWindowLogic.TableInfos[i];
            var tableName = tableInfo.TableName;

            if (!_searchedTableName.IsNullOrEmpty() 
             && !_searchedTableName.Equals(tableName))
            {
                continue;
            }

            _checkToggleTables[i].toggle = GUILayout.Toggle(_checkToggleTables[i].toggle, tableName);
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawBakeSelectedAndBakeAll()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        GUIUtil.Btn("Bake Selected", () =>
        {
            _tableWindowLogic.BakeTable(_checkToggleTables);
        });
        GUIUtil.Btn("Bake All", () =>
        {
            SetCheckToggleTables(true);
            _tableWindowLogic.BakeTable(_checkToggleTables);
        });
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSelectAndDeSelectBtn()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        GUIUtil.Btn("Select All", () => { SetCheckToggleTables(true); });
        GUIUtil.Btn("DeSelect All", () => { SetCheckToggleTables(false); });
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void SetCheckToggleTables(bool select)
    {
        for (var i = 0; i < _checkToggleTables.Length; i++)
        {
            _checkToggleTables[i].toggle = select;
        }
    }
}
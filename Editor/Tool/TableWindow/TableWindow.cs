using UnityEditor;
using UnityEngine;

internal sealed class TableWindow : EditorWindow
{
    public const string TABLE_EXTENSION = ".csv";
    private const float BTN_WIDTH = 200;
    private static readonly string KEY_TABLE_FOLDER_PATH = $"{typeof(TableWindow)}_KEY_TABLE_FOLDER_PATH";
    private static readonly string KEY_SO_CREATION_PATH = $"{typeof(TableWindow)}_KEY_SO_CREATION_PATH";
    private static readonly string KEY_SCRIPT_CREATION_PATH = $"{typeof(TableWindow)}_KEY_SCRIPT_CREATION_PATH";
    
    private readonly TableCodeGenerator _tableCodeGenerator = new();
    private readonly TableWindowLogic _tableWindowLogic = new();
    private (bool toggle, TableWindowLogic.TableInfo tableInfo)[] _checkToggleTables;
    private string _searchedTableName;
    private Vector2 _tableListScrollPos;
    private string _selectedTableFolderPath;
    private string _selectedSOCreationPath;
    private string _selectedScriptCreationPath;
    
    [MenuItem("Custom/Window/TableWindow")]
    public static void Open()
    {
        Initialize();
    }

    private static void Initialize()
    {
        var tool = GetWindow<TableWindow>();
        tool._selectedTableFolderPath = EditorPrefs.GetString(KEY_TABLE_FOLDER_PATH);
        tool._selectedSOCreationPath = EditorPrefs.GetString(KEY_SO_CREATION_PATH);
        tool._selectedScriptCreationPath = EditorPrefs.GetString(KEY_SCRIPT_CREATION_PATH);
        
        if (!tool._selectedTableFolderPath.IsNullOrEmpty())
        {
            tool.CreateTableWindowLogic();
        }
    }
    
    private void CreateTableWindowLogic()
    {
        if (_selectedTableFolderPath.IsNullOrEmpty() || _selectedSOCreationPath.IsNullOrEmpty())
        {
            return;
        }
        
        if (!_tableWindowLogic.Initialize(_selectedTableFolderPath, _selectedSOCreationPath))
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
        DrawSOCreationPath();
        DrawScriptCreationPath();

        if (!_checkToggleTables.IsNullOrEmpty())
        {
            EditorGUILayout.BeginHorizontal();
            DrawTableList();
            DrawSelectedTableInfo();
            EditorGUILayout.EndHorizontal();
        }
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
        const string title = "Table Directory Path";
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUIUtil.Btn(title, BTN_WIDTH, () =>
        {
            var tableFolderPath = EditorUtility.OpenFolderPanel(title, _selectedTableFolderPath, "");
            if (tableFolderPath.IsNullOrEmpty())
            {
                return;
            }

            _selectedTableFolderPath = tableFolderPath;
            CreateTableWindowLogic();
            EditorPrefs.SetString(KEY_TABLE_FOLDER_PATH, _selectedTableFolderPath);
        });
        
        GUILayout.Label(_selectedTableFolderPath);
        GUIUtil.Btn("Refresh", 70, Initialize); 
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawSOCreationPath()
    {
        const string title = "ScriptableObject Creation Path";
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUIUtil.Btn(title, BTN_WIDTH, () =>
        {
            var outputFolderPath = EditorUtility.OpenFolderPanel(title, _selectedSOCreationPath, "");
            if (outputFolderPath.IsNullOrEmpty())
            {
                return;
            }

            _selectedSOCreationPath = outputFolderPath.Replace(Application.dataPath, "Assets");
            CreateTableWindowLogic();
            EditorPrefs.SetString(KEY_SO_CREATION_PATH, _selectedSOCreationPath);
        });
        
        GUILayout.Label(_selectedSOCreationPath);
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawScriptCreationPath()
    {
        const string title = "Script Creation Path";
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        GUIUtil.Btn(title, BTN_WIDTH, () =>
        {
            var outputFolderPath = EditorUtility.OpenFolderPanel(title, _selectedScriptCreationPath, "");
            if (outputFolderPath.IsNullOrEmpty())
            {
                return;
            }

            _selectedScriptCreationPath = outputFolderPath.Replace(Application.dataPath, "Assets");
            CreateTableWindowLogic();
            EditorPrefs.SetString(KEY_SCRIPT_CREATION_PATH, _selectedScriptCreationPath);
        });
        
        GUILayout.Label(_selectedScriptCreationPath);
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
        DrawCodeGenerator();
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

            if (FilterTableName(tableName))
            {
                _checkToggleTables[i].toggle = GUILayout.Toggle(_checkToggleTables[i].toggle, tableName);
            }
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private bool FilterTableName(string tableName)
    {
        if (_searchedTableName.IsNullOrEmpty())
        {
            return true;
        }
        
        var searchedTableNameLower = _searchedTableName.ToLower();
        var tableNameLower = tableName.ToLower();
        
        return tableNameLower.Contains(searchedTableNameLower);
    }
    
    private void DrawCodeGenerator()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        GUIUtil.Btn("Code Generator Selected", () =>
        {
            if (_selectedTableFolderPath.IsNullOrEmpty())
            {
                Debug.LogError("Table directory path is empty.");
                return;
            }

            if (_selectedScriptCreationPath.IsNullOrEmpty())
            {
                Debug.LogError("Script output directory path is empty.");
                return;
            }
            
            _tableCodeGenerator.Execute(_selectedTableFolderPath, _selectedScriptCreationPath, _checkToggleTables);
        });
        
        GUIUtil.Btn("Code Generator All", () =>
        {
            if (_selectedTableFolderPath.IsNullOrEmpty())
            {
                Debug.LogError("Table directory path is empty.");
                return;
            }

            if (_selectedScriptCreationPath.IsNullOrEmpty())
            {
                Debug.LogError("Script output directory path is empty.");
                return;
            }
            
            SetCheckToggleTables(true);
            _tableCodeGenerator.Execute(_selectedTableFolderPath, _selectedScriptCreationPath, _checkToggleTables);
        });
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawBakeSelectedAndBakeAll()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        GUIUtil.Btn("SO Generator Selected", () =>
        {
            _tableWindowLogic.BakeTable(_checkToggleTables);
        });
        GUIUtil.Btn("SO Generator  All", () =>
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
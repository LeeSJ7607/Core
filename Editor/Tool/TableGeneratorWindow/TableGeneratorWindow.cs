using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal sealed class TableGeneratorWindow : EditorWindow
{
    public const string ASSET_GROUP_NAME = "Table";
    public const string TABLE_EXTENSION = ".csv";
    private const float BTN_WIDTH = 200;
    private static readonly string KEY_TABLE_FOLDER_PATH = $"{typeof(TableGeneratorWindow)}_KEY_TABLE_FOLDER_PATH";
    private static readonly string KEY_SO_CREATION_PATH = $"{typeof(TableGeneratorWindow)}_KEY_SO_CREATION_PATH";
    private static readonly string KEY_SCRIPT_CREATION_PATH = $"{typeof(TableGeneratorWindow)}_KEY_SCRIPT_CREATION_PATH";
    
    private readonly TableCodeGenerator _tableCodeGenerator = new();
    private readonly TableGenerator _tableGenerator = new();
    private (bool toggle, TableGenerator.TableInfo tableInfo)[] _checkToggleTables;
    private string _searchedTableName;
    private Vector2 _tableListScrollPos;
    private string _selectedTableFolderPath;
    private string _selectedSOCreationPath;
    private string _selectedScriptCreationPath;
    
    [MenuItem("Custom/Window/TableGeneratorWindow")]
    private static void Open()
    {
        GetWindow<TableGeneratorWindow>().Show();
    }

    private void OnEnable()
    {
        _selectedTableFolderPath = EditorPrefs.GetString(KEY_TABLE_FOLDER_PATH);
        _selectedSOCreationPath = EditorPrefs.GetString(KEY_SO_CREATION_PATH);
        _selectedScriptCreationPath = EditorPrefs.GetString(KEY_SCRIPT_CREATION_PATH);
        
        if (!_selectedTableFolderPath.IsNullOrEmpty())
        {
            CreateTableWindowLogic();
        }
    }

    private void CreateTableWindowLogic()
    {
        if (_selectedTableFolderPath.IsNullOrEmpty() || _selectedSOCreationPath.IsNullOrEmpty())
        {
            return;
        }
        
        if (!_tableGenerator.Initialize(_selectedTableFolderPath, _selectedSOCreationPath))
        {
            _checkToggleTables = null;
            return;
        }

        _checkToggleTables = new (bool, TableGenerator.TableInfo)[_tableGenerator.TableInfos.Length];
            
        for (var i = 0; i < _checkToggleTables.Length; i++)
        {
            _checkToggleTables[i].toggle = false;
            _checkToggleTables[i].tableInfo = _tableGenerator.TableInfos[i];
        }
    }

    private void OnGUI()
    {
        GUIUtil.Btn("Create Table Directory", CreateTableDirectory);
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

            if (!outputFolderPath.Contains(ASSET_GROUP_NAME))
            {
                EditorUtility.DisplayDialog("Error", "The directory must contain the word 'Table'", "OK");
                return;
            }

            if (!IsValidSOCreationPath(outputFolderPath))
            {
                EditorUtility.DisplayDialog("Invalid folder path", GetSOCreationPathExamples(), "OK");
                return;
            }
            
            _selectedSOCreationPath = outputFolderPath.Replace(Application.dataPath, "Assets");
            CreateTableWindowLogic();
            EditorPrefs.SetString(KEY_SO_CREATION_PATH, _selectedSOCreationPath);
        });
        
        GUILayout.Label(_selectedSOCreationPath);
        EditorGUILayout.EndHorizontal();
    }
    
    private IReadOnlyList<string> GetTableDirectoryPaths()
    {
        var rootPath = $"Assets/{ASSET_GROUP_NAME}";
        var paths = new List<string>
        {
            $"{rootPath}/{AddressConst.COMMON_GROUP_NAME}"
        };

        for (var type = EScene.Login; type < EScene.End; type++)
        {
            var path = $"{rootPath}/{type.ToString()}";
            paths.Add(path);
        }
        
        return paths;
    }

    private void CreateTableDirectory()
    {
        var rootPath = $"Assets/{ASSET_GROUP_NAME}";
        if (!AssetDatabase.IsValidFolder(rootPath))
        {
            AssetDatabase.CreateFolder("Assets", ASSET_GROUP_NAME);
        }
        
        var tableDirectoryPaths = GetTableDirectoryPaths();
        foreach (var path in tableDirectoryPaths)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                continue;
            }
            
            var parentFolder = Path.GetDirectoryName(path);
            var newFolderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }
    }

    private bool IsValidSOCreationPath(string outputFolderPath)
    {
        var tableDirectoryPaths = GetTableDirectoryPaths();

        foreach (var path in tableDirectoryPaths)
        {
            if (outputFolderPath.Contains(path))
            {
                return true;
            }
        }
        
        return false;
    }

    private string GetSOCreationPathExamples()
    {
        var msg = string.Empty;
        
        var tableDirectoryPaths = GetTableDirectoryPaths();

        foreach (var path in tableDirectoryPaths)
        {
            msg += $"ex: {path}\n";
        }

        return msg;
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
        if (_tableGenerator.TableInfos.IsNullOrEmpty())
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
        
        for (var i = 0; i < _tableGenerator.TableInfos.Length; i++)
        {
            var tableInfo = _tableGenerator.TableInfos[i];
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
                EditorUtility.DisplayDialog("Error", "Table directory path is empty.", "OK");
                return;
            }

            if (_selectedScriptCreationPath.IsNullOrEmpty())
            {
                EditorUtility.DisplayDialog("Error", "Script output directory path is empty.", "OK");
                return;
            }
            
            _tableCodeGenerator.Execute(_selectedTableFolderPath, _selectedScriptCreationPath, _checkToggleTables);
        });
        
        GUIUtil.Btn("Code Generator All", () =>
        {
            if (_selectedTableFolderPath.IsNullOrEmpty())
            {
                EditorUtility.DisplayDialog("Error", "Table directory path is empty.", "OK");
                return;
            }

            if (_selectedScriptCreationPath.IsNullOrEmpty())
            {
                EditorUtility.DisplayDialog("Error", "Script output directory path is empty.", "OK");
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
            _tableGenerator.BakeTable(_checkToggleTables);
        });
        GUIUtil.Btn("SO Generator All", () =>
        {
            SetCheckToggleTables(true);
            _tableGenerator.BakeTable(_checkToggleTables);
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
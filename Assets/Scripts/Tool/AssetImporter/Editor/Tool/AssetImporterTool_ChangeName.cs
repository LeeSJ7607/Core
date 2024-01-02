using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

public sealed class AssetImporterTool_ChangeName : EditorWindow
{
    private const int _textFieldWidth = 200;
    
    private IEnumerable<AssetImporterImpl_Texture.AssetInfo> _assetInfos;
    private string _originName, _changeName, _addName, _lowerAndUpperName;

    public static void Open(IEnumerable<AssetImporterImpl_Texture.AssetInfo> assetInfos)
    {
        var tool = GetWindow<AssetImporterTool_ChangeName>();
        tool._assetInfos = assetInfos;
    }

    private void OnGUI()
    {
        DrawChangeFileName();
        DrawAddFileName();
        DrawLowerAndUpper();
        DrawRemoveNumber();
    }
    
    private void DrawChangeFileName()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Label("원래 문자열");
                _originName = GUILayout.TextField(_originName, GUIUtil.TextFieldStyle(), GUILayout.Width(_textFieldWidth));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Label("변경할 문자열");
                _changeName = GUILayout.TextField(_changeName, GUIUtil.TextFieldStyle(), GUILayout.Width(_textFieldWidth));
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        
        GUIUtil.Btn("문자열 교체", () =>
        {
            foreach (var assetInfo in _assetInfos)
            {
                var assetPath = assetInfo.Path;
                var path = Path.GetDirectoryName(assetPath);
                var newName = assetInfo.Texture2D.name.Replace(_originName, _changeName);
                var newPath = Path.Combine(path, $"{newName}{Path.GetExtension(assetPath)}");
                AssetDatabase.MoveAsset(assetPath, newPath);
            }
        });
        GUILayout.EndVertical();
    }
    
    private void DrawAddFileName()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        {
            GUILayout.Label("추가할 문자열");
            _addName = GUILayout.TextField(_addName, GUIUtil.TextFieldStyle(), GUILayout.Width(_textFieldWidth * 2));
        }
        GUILayout.EndVertical();
        
        GUILayout.BeginHorizontal();
        {
            GUIUtil.Btn("앞에 추가", () => Add(true));
            GUIUtil.Btn("뒤에 추가", () => Add(false));
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        void Add(bool first)
        {
            foreach (var assetInfo in _assetInfos)
            {
                var assetPath = assetInfo.Path;
                var path = Path.GetDirectoryName(assetPath);
                var newName = first ? $"{_addName}{assetInfo.Texture2D.name}" : $"{assetInfo.Texture2D.name}{_addName}";
                var newPath = Path.Combine(path, $"{newName}{Path.GetExtension(assetPath)}");
                AssetDatabase.MoveAsset(assetPath, newPath);
            }
        }
    }

    private void DrawLowerAndUpper()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginVertical(EditorStyles.helpBox);
        {
            GUILayout.Label("치환할 문자열");
            _lowerAndUpperName = GUILayout.TextField(_lowerAndUpperName, GUIUtil.TextFieldStyle(), GUILayout.Width(_textFieldWidth * 2));
        }
        GUILayout.EndVertical();
        
        GUILayout.BeginHorizontal();
        {
            GUIUtil.Btn("대문자로 변경", () => Change(false));
            GUIUtil.Btn("소문자로 변경", () => Change(true));
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        void Change(bool lower)
        {
            foreach (var assetInfo in _assetInfos)
            {
                var assetPath = assetInfo.Path;
                var path = Path.GetDirectoryName(assetPath);
                var changeName = lower ? _lowerAndUpperName.ToLower() : _lowerAndUpperName.ToUpper();
                var newName = assetInfo.Texture2D.name.ToLower().Replace(_lowerAndUpperName.ToLower(), changeName);
                var newPath = Path.Combine(path, $"{newName}{Path.GetExtension(assetPath)}");
                AssetDatabase.MoveAsset(assetPath, newPath);
            }
        }
    }
    
    private void DrawRemoveNumber()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUIUtil.Btn("숫자 제거", () =>
        {
            foreach (var assetInfo in _assetInfos)
            {
                var assetPath = assetInfo.Path;
                var path = Path.GetDirectoryName(assetPath);
                var newName = Regex.Replace(assetInfo.Texture2D.name, @"\d", "");
                var newPath = Path.Combine(path, $"{newName}{Path.GetExtension(assetPath)}");
                AssetDatabase.MoveAsset(assetInfo.Path, newPath);
            }
        });
        GUILayout.EndVertical();
    }
}
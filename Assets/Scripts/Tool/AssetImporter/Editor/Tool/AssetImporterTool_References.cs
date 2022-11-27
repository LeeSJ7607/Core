using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_References : EditorWindow
{
    private IReadOnlyDictionary<Object, IReadOnlyList<string>> _references;
    private Vector2 _scrollPos;
    
    public static void Open(Object selectedObj)
    {
        var references = AssetImporterUtil.CalcReferences(selectedObj);
        if (references[selectedObj].Count == 0)
        {
            EditorUtility.DisplayDialog("알림", "참조된 에셋이 없습니다.", "확인");
            return;
        }
        
        var tool = GetWindow<AssetImporterTool_References>("References");
        tool._references = references;
    }
    
    public static void Open(Object selectedObj, IReadOnlyDictionary<Object, IReadOnlyList<string>> references)
    {
        if (references[selectedObj].Count == 0)
        {
            EditorUtility.DisplayDialog("알림", "참조된 에셋이 없습니다.", "확인");
            return;
        }
        
        var tool = GetWindow<AssetImporterTool_References>("References");
        tool._references = references;
    }

    private void OnGUI()
    {
        foreach (var pair in _references)
        {
            var target = pair.Key;
            var dependencies = pair.Value;

            DrawTarget(target, dependencies.Count);
            DrawDependencies(dependencies);
        }
    }

    private void DrawTarget(Object target, int cnt)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        EditorGUILayout.ObjectField(target, typeof(Object), true);
        EditorGUILayout.LabelField($"참조된 수: {cnt.ToString()}");
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDependencies(IEnumerable<string> dependencies)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        foreach (var dependency in dependencies)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField(dependency);
                if (GUILayout.Button("선택", GUIUtil.ButtonStyle(), GUILayout.Width(50)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(dependency);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
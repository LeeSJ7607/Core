using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_ReferenceList : EditorWindow
{
    public sealed class ReferenceParam
    {
        public AssetImporterConsts.AssetKind AssetKind { get; }
        public IReadOnlyDictionary<Object, IReadOnlyList<Object>> References { get; }
        public string FileSizeStr { get; }
        
        public ReferenceParam(
            AssetImporterConsts.AssetKind assetKind, 
            IReadOnlyDictionary<Object, IReadOnlyList<Object>> references, 
            string fileSizeStr)
        {
            AssetKind = assetKind;
            References = references;
            FileSizeStr = fileSizeStr;
        }
    }
    
    private ReferenceParam _referenceParam;
    private Vector2 _scrollPos;
    
    public static void Open(ReferenceParam referenceParam)
    {
        var tool = GetWindow<AssetImporterTool_ReferenceList>("References");
        tool._referenceParam = referenceParam;
    }

    private void OnGUI()
    {
        foreach (var (target, dependencies) in _referenceParam.References)
        {
            DrawTarget(target, dependencies.Count);
            DrawDependencies(dependencies);
        }
    }

    private void DrawTarget(Object target, int cnt)
    {
        const float size = 40;
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        if (_referenceParam.AssetKind == AssetImporterConsts.AssetKind.Texture)
        {
            GUIUtil.Btn((Texture2D)target, size, size, () => AssetImporterTool_Preview.Open((Texture2D)target));
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label($"{target.name} ({_referenceParam.FileSizeStr})");
                GUILayout.Label($"참조 수 {cnt.ToString()}");
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label($"{target.name} ({_referenceParam.FileSizeStr}), 참조 수 {cnt.ToString()}");
                EditorGUILayout.ObjectField(target, typeof(Object), true);
            }
            EditorGUILayout.EndVertical();
        }

        GUIUtil.Btn("선택", size, size, () => Selection.activeObject = target);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDependencies(IEnumerable<Object> dependencies)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        foreach (var dependency in dependencies)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.ObjectField(dependency, typeof(Object), true);
                GUIUtil.Btn("선택", () => Selection.activeObject = dependency);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
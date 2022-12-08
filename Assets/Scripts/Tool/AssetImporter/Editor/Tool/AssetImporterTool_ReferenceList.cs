using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_ReferenceList : EditorWindow
{
    private AssetImporter_TextureImpl.AssetInfo _assetInfo;
    private Vector2 _scrollPos;
    
    public static void Open(AssetImporter_TextureImpl.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetImporterTool_ReferenceList>("References");
        tool._assetInfo = assetInfo;
    }

    private void OnGUI()
    {
        foreach (var pair in _assetInfo.References)
        {
            var target = pair.Key;
            var dependencies = pair.Value;

            DrawTarget((Texture2D)target, dependencies.Count);
            DrawDependencies(dependencies);
        }
    }

    private void DrawTarget(Texture2D target, int cnt)
    {
        const float size = 40;

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            GUIUtil.Btn(target, size, size, () => AssetImporterTool_Preview.Open(target));
            DrawTargetDesc(target, cnt);
            GUIUtil.Btn("선택", size, size, () => Selection.activeObject = target);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawTargetDesc(Texture2D target, int cnt)
    {
        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label($"{target.name} ({_assetInfo.FileSize})");
            GUILayout.Label($"참조 수 {cnt.ToString()}");
        }
        EditorGUILayout.EndVertical();
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
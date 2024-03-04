using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_CompareList : EditorWindow
{
    public sealed class CompareParam
    {
        public AssetImporterConsts.AssetKind AssetKind { get; }
        public Object Target { get; }
        public IReadOnlyDictionary<int, DependencyUtil.SameAssetInfo> Compares { get; set; }
        public string FileSizeStr { get; }
        
        public CompareParam(
            AssetImporterConsts.AssetKind assetKind, 
            Object target,
            IReadOnlyDictionary<int, DependencyUtil.SameAssetInfo> compare,
            string fileSizeStr)
        {
            AssetKind = assetKind;
            Target = target;
            Compares = compare;
            FileSizeStr = fileSizeStr;
        }
    }

    private CompareParam _compareParam;
    private Vector2 _scrollPos;
    
    public static void Open(CompareParam compareParam)
    {
        var tool = GetWindow<AssetImporterTool_CompareList>();
        tool._compareParam = compareParam;
    }
    
    private void OnGUI()
    {
        DrawTarget();
        DrawSameAssets();
    }
    
    private void DrawTarget()
    {
        const float size = 40;
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        if (_compareParam.AssetKind == AssetImporterConsts.AssetKind.Texture)
        {
            GUIUtil.Btn((Texture2D)_compareParam.Target, size, size, () => AssetImporterTool_Preview.Open(_compareParam.Target));
            DrawTargetDesc();
            GUIUtil.Btn("선택", size, size, () => Selection.activeObject = _compareParam.Target);
        }
        else
        {
            DrawTargetDesc();
            GUIUtil.Btn("선택", size, size, () => Selection.activeObject = _compareParam.Target);
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawTargetDesc()
    {
        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label($"Name: {_compareParam.Target.name}");
            GUILayout.Label($"FileSize: {_compareParam.FileSizeStr}");
            GUILayout.Label($"동일한 텍스쳐 수: {_compareParam.Compares.Count.ToString()}");
        }
        EditorGUILayout.EndVertical();
    }
    
    private void DrawSameAssets()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        foreach (var (_, sameAsset) in _compareParam.Compares)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.ObjectField(sameAsset.Obj, typeof(Object), true);
                DrawSameAssetBtn(sameAsset);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    
    private void DrawSameAssetBtn(DependencyUtil.SameAssetInfo sameAsset)
    {
        GUIUtil.Btn("비교", () => AssetImporterTool_Compare.Open((Texture2D)_compareParam.Target, (Texture2D)sameAsset.Obj));
        GUIUtil.Btn("텍스쳐 선택", () => Selection.activeObject = sameAsset.Obj);

        if (sameAsset.Mat)
        {
            GUIUtil.Btn("참조된 메테리얼 선택", () => Selection.activeObject = sameAsset.Mat);
        }
        else
        {
            GUIUtil.Btn("참조된 메테리얼 없음", null);
        }
    }
}
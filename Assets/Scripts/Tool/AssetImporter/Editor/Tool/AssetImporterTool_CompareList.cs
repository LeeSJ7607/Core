using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_CompareList : EditorWindow
{
    private AssetImporterImpl_Texture.AssetInfo _assetInfo;
    private Vector2 _scrollPos;
    
    public static void Open(AssetImporterImpl_Texture.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetImporterTool_CompareList>();
        tool._assetInfo = assetInfo;
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
        {
            GUIUtil.Btn(_assetInfo.Texture2D, size, size, () => AssetImporterTool_Preview.Open(_assetInfo.Texture2D));
            DrawTargetDesc();
            GUIUtil.Btn("선택", size, size, () => Selection.activeObject = _assetInfo.Texture2D);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawTargetDesc()
    {
        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label($"Name: {_assetInfo.Texture2D.name}");
            GUILayout.Label($"FileSize: {_assetInfo.FileSizeStr}");
            GUILayout.Label($"동일한 텍스쳐 수: {_assetInfo.Compares.Count.ToString()}");
        }
        EditorGUILayout.EndVertical();
    }
    
    private void DrawSameAssets()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        
        foreach (var (_, sameAsset) in _assetInfo.Compares)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.ObjectField(sameAsset.Tex, typeof(Object), true);
                DrawSameAssetBtn(sameAsset);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    
    private void DrawSameAssetBtn(DependencyUtil.SameAssetInfo sameAsset)
    {
        GUIUtil.Btn("비교", () => AssetImporterTool_Compare.Open(_assetInfo.Texture2D, sameAsset.Tex));
        GUIUtil.Btn("텍스쳐 선택", () => Selection.activeObject = sameAsset.Tex);

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
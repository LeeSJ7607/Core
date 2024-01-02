using UnityEditor;
using UnityEngine;

internal sealed class AssetImporterTool : EditorWindow
{
    private const float _drawMenuBtn = 30;
    private readonly AssetImporter _importer = new();
    
    [MenuItem("Tool/AssetImporterTool &Q")]
    public static void Open()
    {
        var tool = GetWindow<AssetImporterTool>();
        
        // 저장 방식은 스크립터블 오브젝트 하나 두어야할듯.
        // 창을 열때, 기본적으로 보여줄 화면 (이펙터면 이펙터, 유아이면 유아이)
        // 폰트 사이즈, 텍스쳐 크기 등.. 서치 올 참고..
        tool._importer.IsOn = true;
    }
    
    private void OnGUI()
    {
        DrawCategory();
        DrawDesc();
        DrawMenu();
    }

    private void DrawCategory()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button($"텍스쳐 수: {_importer.TextureCnt.ToString()}"))
        {
            ToolOn(_importer);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void ToolOn(AssetImporter importer)
    {
        _importer.IsOn = false;
        importer.IsOn = true;
    }

    private void DrawDesc()
    {
        if (!_importer.IsOn)
        {
            return;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _importer.Draw();
        EditorGUILayout.EndVertical();
    }

    private void DrawMenu()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        DrawDiffBtn();
        DrawConfirmBtn();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawDiffBtn()
    {
        if (GUILayout.Button("변경된 에셋 보기", GUIUtil.ButtonStyle(), GUILayout.Height(_drawMenuBtn)) == false)
        {
            return;
        }

        if (_importer.IsOn)
        {
            _importer.ShowDiff();
        }
    }

    private void DrawConfirmBtn()
    {
        if (GUILayout.Button("변경된 에셋 적용", GUIUtil.ButtonStyle(), GUILayout.Height(_drawMenuBtn)))
        {
            Save();
        }
    }
    
    private void Save()
    {
        var changed = _importer.IsOn && _importer.TrySave();
        var msg = changed ? "변경된 에셋을 적용했습니다." : "변경된 에셋이 없습니다.";
        EditorUtility.DisplayDialog("알림", msg, "확인");
    }
}
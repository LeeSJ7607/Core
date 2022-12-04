using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ToolMode
{
    None,
    Compare
}

internal sealed class AssetImporterTool : EditorWindow
{
    private const float _drawMenuBtn = 30;
    
    public static ToolMode ToolMode { get; set; }
    private readonly IReadOnlyList<AssetImporterPart> _assetImporterParts;

    public AssetImporterTool()
    {
        _assetImporterParts = typeof(AssetImporterPart).Assembly.GetExportedTypes()
                                                       .Where(_ => _.IsInterface == false && _.IsAbstract == false)
                                                       .Where(_ => typeof(AssetImporterPart).IsAssignableFrom(_))
                                                       .Select(_ => (AssetImporterPart)Activator.CreateInstance(_))
                                                       .ToArray();
    }
    
    [MenuItem("THG/AssetImporterTool &Q")]
    public static void Open()
    {
        var tool = GetWindow<AssetImporterTool>();
        
        // 저장 방식은 스크립터블 오브젝트 하나 두어야할듯.
        // 창을 열때, 기본적으로 보여줄 화면 (이펙터면 이펙터, 유아이면 유아이)
        // 폰트 사이즈, 텍스쳐 크기 등.. 서치 올 참고..
        tool._assetImporterParts[0].IsOn = true;
    }

    private void OnDisable()
    {
        Reset();
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
        foreach (var assetImporterPart in _assetImporterParts)
        {
            if (GUILayout.Button(assetImporterPart.Name))
            {
                ToolOn(assetImporterPart);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void ToolOn(AssetImporterPart part)
    {
        foreach (var assetImporterPart in _assetImporterParts)
        {
            assetImporterPart.IsOn = false;
        }

        part.IsOn = true;
    }

    private void DrawDesc()
    {
        foreach (var assetImporterPart in _assetImporterParts)
        {
            if (assetImporterPart.IsOn == false)
            {
                continue;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            assetImporterPart.Draw();
            EditorGUILayout.EndVertical();
        }
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

        foreach (var assetImporterPart in _assetImporterParts)
        {
            if (assetImporterPart.IsOn)
            {
                assetImporterPart.ShowDiff();
            }
        }
    }

    private void DrawConfirmBtn()
    {
        if (GUILayout.Button(ConfirmBtnName(), GUIUtil.ButtonStyle(), GUILayout.Height(_drawMenuBtn)) == false)
        {
            return;
        }

        switch (ToolMode)
        {
        case ToolMode.None:
            {
                Save();
            }
            break;

        default:
            {
                Reset();
            }
            break;
        }
    }

    private string ConfirmBtnName()
    {
        return ToolMode switch
        {
            ToolMode.None => "변경된 에셋 적용",
            ToolMode.Compare => "비교 모드 종료",
        };
    }
    
    private void Save()
    {
        var changed = false;
        
        foreach (var assetImporterPart in _assetImporterParts)
        {
            if (assetImporterPart.IsOn && assetImporterPart.TrySave())
            {
                changed = true;
            }
        }

        var msg = changed ? "변경된 에셋을 적용했습니다." : "변경된 에셋이 없습니다.";
        EditorUtility.DisplayDialog("알림", msg, "확인");
    }
    
    private void Reset()
    {
        End(ToolMode.Compare);
        ToolMode = ToolMode.None;
    }
    
    private void End(ToolMode toolMode)
    {
        foreach (var assetImporterPart in _assetImporterParts)
        {
            assetImporterPart.End(toolMode);
        }
    }
}
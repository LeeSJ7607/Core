using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ToolMode
{
    None,
    Compare,
    References
}

internal sealed class AssetImporterTool : EditorWindow
{
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
        ClearMode();
    }

    private void OnGUI()
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

        DrawToolModeBtn();
    }

    private void DrawToolModeBtn()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        if (GUILayout.Button(GetToolModeBtnName(), GUIUtil.ButtonStyle(), GUILayout.Height(50)))
        {
            switch (ToolMode)
            {
            case ToolMode.None:
                {
                    Save();
                }
                break;

            default:
                {
                    ClearMode();
                }
                break;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ClearMode()
    {
        End(ToolMode.Compare);
        End(ToolMode.References);
        ToolMode = ToolMode.None;
    }
    
    private string GetToolModeBtnName()
    {
        return ToolMode switch
        {
            ToolMode.None => "변경된 사항 적용",
            ToolMode.Compare => "비교 모드 종료",
            ToolMode.References => "참조 모드 종료",
        };
    }

    private void ToolOn(AssetImporterPart part)
    {
        foreach (var assetImporterPart in _assetImporterParts)
        {
            assetImporterPart.IsOn = false;
        }

        part.IsOn = true;
    }

    private void End(ToolMode toolMode)
    {
        foreach (var assetImporterPart in _assetImporterParts)
        {
            assetImporterPart.End(toolMode);
        }
    }

    private void Save()
    {
        var changed = false;
        
        foreach (var assetImporterPart in _assetImporterParts)
        {
            if (assetImporterPart.TrySave())
            {
                changed = true;
            }
        }

        var msg = changed ? "변경된 사항을 적용했습니다." : "변경된 사항이 없습니다.";
        EditorUtility.DisplayDialog("알림", msg, "확인");
    }
}
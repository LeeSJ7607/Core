using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal sealed class AssetImporterTool : EditorWindow
{
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
        
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button("변경된 사항 적용", GUIUtil.ButtonStyle(), GUILayout.Height(50)))
        {
            Save();
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
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

internal sealed class AssetImporterTool_FBXModify : EditorWindow
{
    private const float _toolWidth = 500;
    
    private static readonly string[] _normalTypes = Enum.GetNames(typeof(ModelImporterNormals)).ToArray();
    private int _selectedNormalTypesIdx;
    private int _originNormalTypesIdx;
    
    private static readonly string[] _tangentTypes = Enum.GetNames(typeof(ModelImporterTangents)).ToArray();
    private int _selectedTangentTypesIdx;
    private int _originTangentTypesIdx;
    
    private static readonly string[] _meshCompressionTypes = Enum.GetNames(typeof(ModelImporterMeshCompression)).ToArray();
    private int _selectedMeshCompressionTypesIdx;
    private int _originMeshCompressionTypesIdx;
    
    private static readonly string[] _isReadable =
    {
        "True",
        "False",
    };
    private int _selectedIsReadableIdx;
    private int _originIsReadableIdx;
    
    private AssetImporterImpl_FBX.AssetInfo _assetInfo;
    
    public static void Open(AssetImporterImpl_FBX.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetImporterTool_FBXModify>("Modify");
        tool.minSize = tool.maxSize = new Vector2(_toolWidth, 200);
        tool._assetInfo = assetInfo;
        
        SetOption(tool);
    }
    
    private static void SetOption(AssetImporterTool_FBXModify tool)
    {
        var assetInfo = tool._assetInfo;
        
        tool._originNormalTypesIdx = tool._selectedNormalTypesIdx = Array.FindIndex(_normalTypes, _ => _.Equals(assetInfo.Normals.ToString()));
        tool._originTangentTypesIdx = tool._selectedTangentTypesIdx = Array.FindIndex(_tangentTypes, _ => _.Equals(assetInfo.Tangents.ToString()));
        tool._originMeshCompressionTypesIdx = tool._selectedMeshCompressionTypesIdx = Array.FindIndex(_meshCompressionTypes, _ => _.Equals(assetInfo.MeshCompression.ToString()));
        tool._originIsReadableIdx = tool._selectedIsReadableIdx = Array.FindIndex(_isReadable, _ => _.Equals(assetInfo.IsReadable.ToString()));
    }
    
    private void ReSetOption()
    {
        _selectedNormalTypesIdx = _originNormalTypesIdx;
        _selectedTangentTypesIdx = _originTangentTypesIdx;
        _selectedMeshCompressionTypesIdx = _originMeshCompressionTypesIdx;
        _selectedIsReadableIdx = _originIsReadableIdx;
    }
    
    private void OnGUI()
    {
        DrawTitle();
        DrawOption();
        DrawMenus();
    }

    private void DrawTitle()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            GUILayout.Label($"{_assetInfo.ModelImporter.assetPath} ({_assetInfo.FileSizeStr})", GUIUtil.LabelStyle(TextAnchor.MiddleLeft));
        }
        EditorGUILayout.EndVertical();
    }
    
    private void DrawOption()
    {
        GUIUtil.DrawPopup("Mesh Compression", ref _selectedMeshCompressionTypesIdx, _meshCompressionTypes);
        GUIUtil.DrawPopup("Normal", ref _selectedNormalTypesIdx, _normalTypes);
        GUIUtil.DrawPopup("Tangent", ref _selectedTangentTypesIdx, _tangentTypes);
        GUIUtil.DrawPopup("Read/Write", ref _selectedIsReadableIdx, _isReadable);
    }
    
    private void DrawMenus()
    {
        const float width = 100;
        const float guiSpace = 10;
        
        EditorGUILayout.Space(guiSpace);
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.BtnExpand("선택", width, () => Selection.activeObject = _assetInfo.FBX);
            GUIUtil.BtnExpand("열기", width, () => EditorUtility.RevealInFinder(_assetInfo.ModelImporter.assetPath));
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.BtnExpand("저장", width, Save);
            GUIUtil.BtnExpand("설정 값 되돌리기", width, ReSetOption);
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void Save()
    {
        var changed = IsChanged();
        if (!changed)
        {
            EditorUtility.DisplayDialog("알림", "변경된 사항이 없습니다.", "확인");
            return;
        }

        changed = !IsOriginChanged();
        if (!changed)
        {
            EditorUtility.DisplayDialog("알림", "변경전 사항과 동일합니다.", "확인");
            return;
        }

        _assetInfo.Changed = true;
        _assetInfo.Normals = Enum.Parse<ModelImporterNormals>(_normalTypes[_selectedNormalTypesIdx]);
        _assetInfo.Tangents = Enum.Parse<ModelImporterTangents>(_tangentTypes[_selectedTangentTypesIdx]);
        _assetInfo.MeshCompression = Enum.Parse<ModelImporterMeshCompression>(_meshCompressionTypes[_selectedMeshCompressionTypesIdx]);
        _assetInfo.IsReadable = bool.Parse(_isReadable[_selectedIsReadableIdx]);
        
        Close();
    }
    
    private bool IsChanged()
    {
        return _selectedMeshCompressionTypesIdx != _originMeshCompressionTypesIdx 
            || _selectedNormalTypesIdx != _originNormalTypesIdx
            || _selectedTangentTypesIdx != _originTangentTypesIdx 
            || _selectedIsReadableIdx != _originIsReadableIdx; 
    }
    
    private bool IsOriginChanged()
    {
        var modelImporter = _assetInfo.ModelImporter;
        var meshCompression = Enum.Parse<ModelImporterMeshCompression>(_meshCompressionTypes[_selectedMeshCompressionTypesIdx]);
        var normals = Enum.Parse<ModelImporterNormals>(_normalTypes[_selectedNormalTypesIdx]);
        var tangents = Enum.Parse<ModelImporterTangents>(_tangentTypes[_selectedTangentTypesIdx]);
        var isReadable = bool.Parse(_isReadable[_selectedIsReadableIdx]);

        return meshCompression == modelImporter.meshCompression
            && normals == modelImporter.importNormals
            && tangents == modelImporter.importTangents
            && isReadable == modelImporter.isReadable;
    }
}
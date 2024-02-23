using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_SoundModify : EditorWindow
{
    private const float _toolWidth = 500;
    
    private static readonly string[] _compressionFormats = Enum.GetNames(typeof(AudioCompressionFormat)).ToArray();
    private int _selectedCompressionFormatsIdx;
    private int _originCompressionFormatsIdx;
    
    private static readonly string[] _loadTypes = Enum.GetNames(typeof(AudioClipLoadType)).ToArray();
    private int _selectedLoadTypesIdx;
    private int _originLoadTypesIdx;
    
    private static readonly string[] _preloadAudioData =
    {
        "True",
        "False",
    };
    private int _selectedPreloadAudioDataIdx;
    private int _originPreloadAudioDataIdx;
    
    private static readonly string[] _forceToMono =
    {
        "True",
        "False",
    };
    private int _selectedForceToMonoIdx;
    private int _originForceToMonoIdx;
    
    private AssetImporterImpl_Sound.AssetInfo _assetInfo;
    
    public static void Open(AssetImporterImpl_Sound.AssetInfo assetInfo)
    {
        var tool = GetWindow<AssetImporterTool_SoundModify>("Modify");
        tool.minSize = tool.maxSize = new Vector2(_toolWidth, 200);
        tool._assetInfo = assetInfo;
        
        SetOption(tool);
    }
    
    private static void SetOption(AssetImporterTool_SoundModify tool)
    {
        var assetInfo = tool._assetInfo;
        
        tool._originCompressionFormatsIdx = tool._selectedCompressionFormatsIdx = Array.FindIndex(_compressionFormats, _ => _.Equals(assetInfo.CompressionFormat.ToString()));
        tool._originLoadTypesIdx = tool._selectedLoadTypesIdx = Array.FindIndex(_loadTypes, _ => _.Equals(assetInfo.LoadType.ToString()));
        tool._originPreloadAudioDataIdx = tool._selectedPreloadAudioDataIdx = Array.FindIndex(_preloadAudioData, _ => _.Equals(assetInfo.PreloadAudioData.ToString()));
        tool._originForceToMonoIdx = tool._selectedForceToMonoIdx = Array.FindIndex(_forceToMono, _ => _.Equals(assetInfo.ForceToMono.ToString()));
    }
    
    private void ReSetOption()
    {
        _selectedCompressionFormatsIdx = _originCompressionFormatsIdx;
        _selectedLoadTypesIdx = _originLoadTypesIdx;
        _selectedPreloadAudioDataIdx = _originPreloadAudioDataIdx;
        _selectedForceToMonoIdx = _originForceToMonoIdx;
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
            GUILayout.Label($"{_assetInfo.AudioImporter.assetPath} ({_assetInfo.FileSizeStr})", GUIUtil.LabelStyle(TextAnchor.MiddleLeft));
        }
        EditorGUILayout.EndVertical();
    }
    
    private void DrawOption()
    {
        GUIUtil.DrawPopup("AudioCompressionFormat", ref _selectedCompressionFormatsIdx, _compressionFormats);
        GUIUtil.DrawPopup("AudioClipLoadType", ref _selectedLoadTypesIdx, _loadTypes);
        GUIUtil.DrawPopup("PreloadAudioData", ref _selectedPreloadAudioDataIdx, _preloadAudioData);
        GUIUtil.DrawPopup("ForceToMono", ref _selectedForceToMonoIdx, _forceToMono);
    }
    
    private void DrawMenus()
    {
        const float width = 100;
        const float guiSpace = 10;
        
        EditorGUILayout.Space(guiSpace);
        EditorGUILayout.BeginHorizontal();
        {
            GUIUtil.BtnExpand("선택", width, () => Selection.activeObject = _assetInfo.AudioClip);
            GUIUtil.BtnExpand("열기", width, () => EditorUtility.RevealInFinder(_assetInfo.AudioImporter.assetPath));
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
        _assetInfo.CompressionFormat = Enum.Parse<AudioCompressionFormat>(_compressionFormats[_selectedCompressionFormatsIdx]);
        _assetInfo.LoadType = Enum.Parse<AudioClipLoadType>(_loadTypes[_selectedLoadTypesIdx]);
        _assetInfo.PreloadAudioData = bool.Parse(_preloadAudioData[_selectedPreloadAudioDataIdx]);
        _assetInfo.ForceToMono = bool.Parse(_forceToMono[_selectedForceToMonoIdx]);
        
        Close();
    }
    
    private bool IsChanged()
    {
        return _selectedCompressionFormatsIdx != _originCompressionFormatsIdx 
            || _selectedLoadTypesIdx != _originLoadTypesIdx
            || _selectedPreloadAudioDataIdx != _originPreloadAudioDataIdx
            || _selectedForceToMonoIdx != _originForceToMonoIdx;
    }
    
    private bool IsOriginChanged()
    {
        var audioImporter = _assetInfo.AudioImporter;
        var compressionFormat = Enum.Parse<AudioCompressionFormat>(_compressionFormats[_selectedCompressionFormatsIdx]);
        var loadType = Enum.Parse<AudioClipLoadType>(_loadTypes[_selectedLoadTypesIdx]);
        var preloadAudioData = bool.Parse(_preloadAudioData[_selectedPreloadAudioDataIdx]);
        var forceToMono = bool.Parse(_forceToMono[_selectedForceToMonoIdx]);

        return compressionFormat == _assetInfo.CompressionFormat 
            && loadType == _assetInfo.LoadType 
            && preloadAudioData == _assetInfo.PreloadAudioData 
            && forceToMono == audioImporter.forceToMono;
    }
}
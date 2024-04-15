using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public sealed class AssetManagementGUI_Sound : IAssetManagementGUI
{
    private const int _drawMaxRow = 5;
    
    public int Order => 2;
    public int TotalCnt => _soundImpl.TotalCnt;
    public Vector2 ScrollPos { get; set; }
    public IAssetManagementImpl OriginAssetManagementImpl => _originSoundImpl;
    public IAssetManagementImpl AssetManagementImpl => _soundImpl;
    private readonly AssetManagementImpl_Sound _originSoundImpl = new();
    private readonly AssetManagementImpl_Sound _soundImpl = new();
    private int _selectedSoundPathIdx;
    private int _selectedSoundSortIdx;
    private readonly string[] _sortSound = Enum.GetNames(typeof(AssetManagementConsts.SortSound));
    private List<string> _soundDirPaths;
    private List<string> _btnNameSoundDirPaths;
    private Texture2D _texModified;
    
    private void Clear()
    {
        _soundDirPaths?.Clear();
        _btnNameSoundDirPaths?.Clear();
    }
    
    public void Initialize(string selectedFilePath)
    {
        Clear();
        AssetManagementGUI_Texture.SetDirPath(selectedFilePath, "t:AudioClip", ref _soundDirPaths, ref _btnNameSoundDirPaths);
        
        if (_soundDirPaths != null)
        {
            _originSoundImpl.Initialize(_soundDirPaths);
            _soundImpl.Initialize(_soundDirPaths);
        }
        
        _texModified ??= Resources.Load<Texture2D>("AssetManagementTool_Modified");
    }
    
    private bool IsValid()
    {
        return _btnNameSoundDirPaths is {Count: > 0};
    }
    
    public void Draw()
    {
        if (!IsValid())
        {
            return;
        }
        
        DrawFolder();
        DrawMenus();
        DrawAssets();
    }
    
    private void DrawFolder()
    {
        EditorGUILayout.BeginHorizontal();

        for (var i = 0; i < _btnNameSoundDirPaths.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            
            var cnt = _soundImpl.SearchedCnt(_soundDirPaths[i]);
            var toggleName = $"{_btnNameSoundDirPaths[i]} ({cnt.ToString()})";
            
            if (GUILayout.Toggle(_selectedSoundPathIdx == i, toggleName, GUILayout.ExpandWidth(true)))
            {
                if (_selectedSoundPathIdx != i)
                {
                    ScrollPos = Vector2.zero;
                }

                _selectedSoundPathIdx = i;
                _soundImpl.CalcSearchedAssetInfos(_soundDirPaths[_selectedSoundPathIdx]);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawMenus()
    {
        GUIUtil.Btn("모든 참조 찾기", () =>
        {
            DependencyUtil.Dependencies(_soundImpl.SearchedAssetInfos);
            Sort((int)AssetManagementConsts.SortSound.References, true);
        });
        
        DrawSort();
    }
    
    private void DrawSort()
    {
        EditorGUILayout.BeginHorizontal();
        GUIUtil.DrawPopup("정렬", ref _selectedSoundSortIdx, _sortSound, () => Sort(_selectedSoundSortIdx, false));
        GUIUtil.Btn("▲", 25, () => Sort(_selectedSoundSortIdx, false));
        GUIUtil.Btn("▼", 25, () => Sort(_selectedSoundSortIdx, true));
        EditorGUILayout.EndHorizontal();
    }
    
    private void Sort(int sortIdx, bool descending)
    {
        _soundImpl.CurSort = ((AssetManagementConsts.SortSound)sortIdx, descending);
        _soundImpl.CalcSearchedAssetInfos(_soundDirPaths[_selectedSoundPathIdx]);
    }
    
    private void DrawAssets()
    {
        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
        
        var totalCnt = _soundImpl.SearchedAssetInfos.Count;
        for (var i = 0; i < totalCnt; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var j = 0; j < _drawMaxRow; j++)
            {
                var idx = i + j;
                if (idx >= totalCnt)
                {
                    break;
                }
                
                var assetInfo = _soundImpl.SearchedAssetInfos[idx];
                EditorGUILayout.BeginHorizontal(GUIUtil.HelpBoxStyle(assetInfo.Changed ? _texModified : null), GUILayout.Width(375));
                DrawDesc(assetInfo);
                DrawOption(assetInfo);
                EditorGUILayout.EndHorizontal();
            }
            
            i += (_drawMaxRow - 1);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawDesc(AssetManagementImpl_Sound.AssetInfo assetInfo)
    {
        const float keyWidth = 120;
        const float valueWidth = 210;
        
        EditorGUILayout.Space(1);
        GUILayout.BeginVertical();
        
        GUIUtil.Desc("Name", assetInfo.AudioClip.name, keyWidth, valueWidth);
        GUIUtil.Desc("ForceToMono", assetInfo.ForceToMono ? "O" : "X", keyWidth, valueWidth);
        GUIUtil.Desc("PreloadAudioData", assetInfo.PreloadAudioData ? "O" : "X", keyWidth, valueWidth);
        GUIUtil.Desc("CompressionFormat", assetInfo.CompressionFormat.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("SampleRateSetting", assetInfo.SampleRateSetting.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("LoadType", assetInfo.LoadType.ToString(), keyWidth, valueWidth);
        GUIUtil.Desc("File Size", assetInfo.FileSizeStr, keyWidth, valueWidth);
        
        GUILayout.EndVertical();
    }
    
    private void DrawOption(AssetManagementImpl_Sound.AssetInfo assetInfo)
    {
        const float width = 50;
        
        EditorGUILayout.BeginVertical();
        
        GUIUtil.Btn("선택", width, () => Selection.activeObject = assetInfo.AudioClip);
        GUIUtil.Btn("열기", width, () => EditorUtility.RevealInFinder(assetInfo.AudioImporter.assetPath));
        GUIUtil.Btn("수정", width, () => AssetManagementTool_SoundModify.Open(assetInfo));
        GUIUtil.Btn("리셋", width, assetInfo.Reset);
        
        if (assetInfo.IsReferences)
        {
            GUIUtil.Btn("참조", () =>
            {
                AssetManagementTool_ReferenceList.Open(new AssetManagementTool_ReferenceList.ReferenceParam(
                    AssetManagementConsts.AssetKind.Sound, assetInfo.References, assetInfo.FileSizeStr));
            });
        }
        
        EditorGUILayout.EndVertical();
    }
    
    public bool CanDiff() => _soundImpl.CanDiff();
    public bool TrySave() => _soundImpl.TrySave();
}
using UnityEditor;
using UnityEngine;

public sealed class AssetManagementTool_Diff : EditorWindow
{
    private enum EAssetType
    {
        All,
        Texture,
        FBX,
        Sound,
        End,
    }

    private sealed class AssetInfo
    {
        public AssetManagementImpl_Texture TextureImpl { get; }
        public AssetManagementImpl_FBX FBXImpl { get; }
        public AssetManagementImpl_Sound SoundImpl { get; }

        public AssetInfo(
            AssetManagementImpl_Texture textureImpl = null,
            AssetManagementImpl_FBX fbxImpl = null,
            AssetManagementImpl_Sound soundImpl = null)
        {
            TextureImpl = textureImpl;
            FBXImpl = fbxImpl;
            SoundImpl = soundImpl;
        }
    }

    private IAssetManagementGUI[] _assetManagementGuis;
    private AssetInfo _before, _after;
    private EAssetType _curAssetType;
    private Vector2 _scrollPos;

    public static void Open(IAssetManagementGUI[] assetManagementGuis) 
    {
        var tool = GetWindow<AssetManagementTool_Diff>("Diff");
        tool.minSize = tool.maxSize = new Vector2(700, 800);
        tool._assetManagementGuis = assetManagementGuis;
        tool._before = new AssetInfo(
            (AssetManagementImpl_Texture)assetManagementGuis[(int)AssetManagementConsts.EAssetKind.Texture].OriginAssetManagementImpl, 
            (AssetManagementImpl_FBX)assetManagementGuis[(int)AssetManagementConsts.EAssetKind.FBX].OriginAssetManagementImpl,
            (AssetManagementImpl_Sound)assetManagementGuis[(int)AssetManagementConsts.EAssetKind.Sound].OriginAssetManagementImpl);
        tool._after = new AssetInfo(
            (AssetManagementImpl_Texture)assetManagementGuis[(int)AssetManagementConsts.EAssetKind.Texture].AssetManagementImpl, 
            (AssetManagementImpl_FBX)assetManagementGuis[(int)AssetManagementConsts.EAssetKind.FBX].AssetManagementImpl,
            (AssetManagementImpl_Sound)assetManagementGuis[(int)AssetManagementConsts.EAssetKind.Sound].AssetManagementImpl);
        
        Sync(tool);
    }

    private static void Sync(AssetManagementTool_Diff diff)
    {
        var beforeTextureAssetInfoMap = diff._before.TextureImpl.AssetInfoMap;
        var afterTextureAssetInfoMap = diff._after.TextureImpl.AssetInfoMap;
        foreach (var (path, beforeTextures) in beforeTextureAssetInfoMap)
        {
            for (var i = 0; i < beforeTextures.Count; i++)
            {
                beforeTextures[i].Changed = afterTextureAssetInfoMap[path][i].Changed;
            }
        }
        
        var beforeFBXAssetInfoMap = diff._before.FBXImpl.AssetInfoMap;
        var afterFBXAssetInfoMap = diff._after.FBXImpl.AssetInfoMap;
        foreach (var (path, beforeTextures) in beforeFBXAssetInfoMap)
        {
            for (var i = 0; i < beforeTextures.Count; i++)
            {
                beforeTextures[i].Changed = afterFBXAssetInfoMap[path][i].Changed;
            }
        }
        
        var beforeSoundAssetInfoMap = diff._before.SoundImpl.AssetInfoMap;
        var afterSoundAssetInfoMap = diff._after.SoundImpl.AssetInfoMap;
        foreach (var (path, beforeTextures) in beforeSoundAssetInfoMap)
        {
            for (var i = 0; i < beforeTextures.Count; i++)
            {
                beforeTextures[i].Changed = afterSoundAssetInfoMap[path][i].Changed;
            }
        }
    }
    
    private void OnDisable()
    {
        _curAssetType = EAssetType.All;
    }

    private void OnGUI()
    {
        DrawCategory();
        DrawAssets();
        DrawConfirmBtn();
    }

    private void DrawCategory()
    {
        EditorGUILayout.BeginHorizontal();
        for (var type = EAssetType.All; type < EAssetType.End; type++)
        {
            if (CanDrawCategory(type))
            {
                GUIUtil.Btn(type.ToString(), () => _curAssetType = type);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool CanDrawCategory(EAssetType type)
    {
        return type switch
        {
            EAssetType.Texture => _after.TextureImpl?.CanDiff() ?? false,
            EAssetType.FBX => _after.FBXImpl?.CanDiff() ?? false,
            EAssetType.Sound => _after.SoundImpl?.CanDiff() ?? false,
            _ => true
        };
    }

    private void DrawAssets()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.BeginHorizontal();
        
        Set(_before);
        Set(_after);
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }
    
    private void Set(AssetInfo assetInfo)
    {
        EditorGUILayout.BeginVertical();
        
        switch (_curAssetType)
        {
        case EAssetType.All:
            {
                DrawTextureImpl(assetInfo.TextureImpl);
                DrawFBXImpl(assetInfo.FBXImpl);
                DrawSoundImpl(assetInfo.SoundImpl);
            }
            break;

        case EAssetType.Texture:
            {
                DrawTextureImpl(assetInfo.TextureImpl);
            }
            break;

        case EAssetType.FBX:
            {
                DrawFBXImpl(assetInfo.FBXImpl);
            }
            break;

        case EAssetType.Sound:
            {
                DrawSoundImpl(assetInfo.SoundImpl);
            }
            break;
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawTextureImpl(AssetManagementImpl_Texture textureImpl)
    {
        if (textureImpl == null)
        {
            return;
        }

        foreach (var (path, assetInfos) in textureImpl.AssetInfoMap)
        {
            for (var i = 0; i < assetInfos.Count; i++)
            {
                var assetInfo = assetInfos[i];
                if (!assetInfo.Changed)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUIUtil.Btn(assetInfo.Texture2D, 50, 50, () => AssetManagementTool_Preview.Open(assetInfo.Texture2D));
                DrawTextureImplDesc(assetInfo, _before.TextureImpl.AssetInfoMap[path][i], _after.TextureImpl.AssetInfoMap[path][i]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    
    private void DrawTextureImplDesc(
        AssetManagementImpl_Texture.AssetInfo assetInfo, 
        AssetManagementImpl_Texture.AssetInfo left, 
        AssetManagementImpl_Texture.AssetInfo right)
    {
        const float keyWidth = 80;
        const float valueWidth = 195;
        var tex = assetInfo.Texture2D;
        
        EditorGUILayout.Space(1);
        GUILayout.BeginVertical();
        GUIUtil.Desc("Name", tex.name, keyWidth, valueWidth);
        GUIUtil.Desc("Texture Type", assetInfo.TextureType.ToString(), keyWidth, valueWidth, left.TextureType, right.TextureType);
        GUIUtil.Desc("Wrap Mode", assetInfo.WrapMode.ToString(), keyWidth, valueWidth, left.WrapMode, right.WrapMode);
        GUIUtil.Desc("Filter Mode", assetInfo.FilterMode.ToString(), keyWidth, valueWidth, left.FilterMode, right.FilterMode);
        GUIUtil.Desc("Max Size", assetInfo.MaxTextureSize.ToString(), keyWidth, valueWidth, left.MaxTextureSize, right.MaxTextureSize);
        GUIUtil.Desc("Format", assetInfo.FormatType.ToString(), keyWidth, valueWidth, left.AOSSettings.format, right.AOSSettings.format);
        GUIUtil.Desc("Texture Size", $"{tex.width.ToString()}x{tex.height.ToString()}", keyWidth, valueWidth);
        GUILayout.EndVertical();
    }
    
    private void DrawFBXImpl(AssetManagementImpl_FBX fbxImpl)
    {
        if (fbxImpl == null)
        {
            return;
        }
        
        const float keyWidth = 110;
        const float valueWidth = 220;

        foreach (var (path, assetInfos) in fbxImpl.AssetInfoMap)
        {
            for (var i = 0; i < assetInfos.Count; i++)
            {
                var assetInfo = assetInfos[i];
                if (!assetInfo.Changed)
                {
                    continue;
                }
                
                var left = _before.FBXImpl.AssetInfoMap[path][i]; 
                var right = _after.FBXImpl.AssetInfoMap[path][i];
                
                EditorGUILayout.Space(1);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUIUtil.Desc("Name", assetInfo.FBX.name, keyWidth, valueWidth);
                GUIUtil.Desc("Normals", assetInfo.Normals.ToString(), keyWidth, valueWidth, left.Normals, right.Normals);
                GUIUtil.Desc("Tangents", assetInfo.Tangents.ToString(), keyWidth, valueWidth, left.Tangents, right.Tangents);
                GUIUtil.Desc("MeshCompression", assetInfo.MeshCompression.ToString(), keyWidth, valueWidth, left.MeshCompression, right.MeshCompression);
                GUIUtil.Desc("Read/Write", assetInfo.IsReadable ? "O" : "X", keyWidth, valueWidth, left.IsReadable, right.IsReadable);
                GUIUtil.Desc("File Size", assetInfo.FileSizeStr, keyWidth, valueWidth);
                GUILayout.EndVertical();
            }
        }
    }
    
    private void DrawSoundImpl(AssetManagementImpl_Sound soundImpl)
    {
        if (soundImpl == null)
        {
            return;
        }
        
        const float keyWidth = 120;
        const float valueWidth = 210;

        foreach (var (path, assetInfos) in soundImpl.AssetInfoMap)
        {
            for (var i = 0; i < assetInfos.Count; i++)
            {
                var assetInfo = assetInfos[i];
                if (!assetInfo.Changed)
                {
                    continue;
                }

                var left = _before.SoundImpl.AssetInfoMap[path][i];
                var right = _after.SoundImpl.AssetInfoMap[path][i];
                
                EditorGUILayout.Space(1);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUIUtil.Desc("Name", assetInfo.AudioClip.name, keyWidth, valueWidth);
                GUIUtil.Desc("ForceToMono", assetInfo.ForceToMono ? "O" : "X", keyWidth, valueWidth, left.ForceToMono, right.ForceToMono);
                GUIUtil.Desc("PreloadAudioData", assetInfo.PreloadAudioData ? "O" : "X", keyWidth, valueWidth, left.PreloadAudioData, right.PreloadAudioData);
                GUIUtil.Desc("CompressionFormat", assetInfo.CompressionFormat.ToString(), keyWidth, valueWidth, left.CompressionFormat, right.CompressionFormat);
                GUIUtil.Desc("SampleRateSetting", assetInfo.SampleRateSetting.ToString(), keyWidth, valueWidth, left.SampleRateSetting, right.SampleRateSetting);
                GUIUtil.Desc("LoadType", assetInfo.LoadType.ToString(), keyWidth, valueWidth, left.LoadType, right.LoadType);
                GUIUtil.Desc("File Size", assetInfo.FileSizeStr, keyWidth, valueWidth);
                GUILayout.EndVertical();
            }
        }
    }
    
    private void DrawConfirmBtn()
    {
        if (!GUILayout.Button("저장", GUIUtil.ButtonStyle(), GUILayout.Height(30)))
        {
            return;
        }

        EditorUtility.DisplayDialog("알림", "변경된 에셋을 적용했습니다.", "확인");
        foreach (var assetManagementGUI in _assetManagementGuis)
        {
            assetManagementGUI.TrySave();
        }
        
        Close();
    }
}
using UnityEditor;
using UnityEngine;

public sealed class AssetImporterTool_Diff : EditorWindow
{
    private enum AssetType
    {
        All,
        Texture,
        FBX,
        Sound,
        End,
    }

    private sealed class AssetInfo
    {
        public AssetImporterImpl_Texture TextureImpl { get; }
        public AssetImporterImpl_FBX FBXImpl { get; }
        public AssetImporterImpl_Sound SoundImpl { get; }

        public AssetInfo(
            AssetImporterImpl_Texture textureImpl = null,
            AssetImporterImpl_FBX fbxImpl = null,
            AssetImporterImpl_Sound soundImpl = null)
        {
            TextureImpl = textureImpl;
            FBXImpl = fbxImpl;
            SoundImpl = soundImpl;
        }
    }

    private IAssetImporterGUI[] _assetImporterGuis;
    private AssetInfo _before, _after;
    private AssetType _curAssetType;
    private Vector2 _scrollPos;

    public static void Open(IAssetImporterGUI[] assetImporterGuis) 
    {
        var tool = GetWindow<AssetImporterTool_Diff>("Diff");
        tool.minSize = tool.maxSize = new Vector2(700, 800);
        tool._assetImporterGuis = assetImporterGuis;
        tool._before = new AssetInfo(
            (AssetImporterImpl_Texture)assetImporterGuis[(int)AssetImporterConsts.AssetKind.Texture].OriginAssetImporterImpl, 
            (AssetImporterImpl_FBX)assetImporterGuis[(int)AssetImporterConsts.AssetKind.FBX].OriginAssetImporterImpl,
            (AssetImporterImpl_Sound)assetImporterGuis[(int)AssetImporterConsts.AssetKind.Sound].OriginAssetImporterImpl);
        tool._after = new AssetInfo(
            (AssetImporterImpl_Texture)assetImporterGuis[(int)AssetImporterConsts.AssetKind.Texture].AssetImporterImpl, 
            (AssetImporterImpl_FBX)assetImporterGuis[(int)AssetImporterConsts.AssetKind.FBX].AssetImporterImpl,
            (AssetImporterImpl_Sound)assetImporterGuis[(int)AssetImporterConsts.AssetKind.Sound].AssetImporterImpl);
        
        Sync(tool);
    }

    private static void Sync(AssetImporterTool_Diff diff)
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
        _curAssetType = AssetType.All;
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
        for (var type = AssetType.All; type < AssetType.End; type++)
        {
            if (CanDrawCategory(type))
            {
                GUIUtil.Btn(type.ToString(), () => _curAssetType = type);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool CanDrawCategory(AssetType type)
    {
        return type switch
        {
            AssetType.Texture => _after.TextureImpl?.CanDiff() ?? false,
            AssetType.FBX => _after.FBXImpl?.CanDiff() ?? false,
            AssetType.Sound => _after.SoundImpl?.CanDiff() ?? false,
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
        case AssetType.All:
            {
                DrawTextureImpl(assetInfo.TextureImpl);
                DrawFBXImpl(assetInfo.FBXImpl);
                DrawSoundImpl(assetInfo.SoundImpl);
            }
            break;

        case AssetType.Texture:
            {
                DrawTextureImpl(assetInfo.TextureImpl);
            }
            break;

        case AssetType.FBX:
            {
                DrawFBXImpl(assetInfo.FBXImpl);
            }
            break;

        case AssetType.Sound:
            {
                DrawSoundImpl(assetInfo.SoundImpl);
            }
            break;
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawTextureImpl(AssetImporterImpl_Texture textureImpl)
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
                GUIUtil.Btn(assetInfo.Texture2D, 50, 50, () => AssetImporterTool_Preview.Open(assetInfo.Texture2D));
                DrawTextureImplDesc(assetInfo, _before.TextureImpl.AssetInfoMap[path][i], _after.TextureImpl.AssetInfoMap[path][i]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    
    private void DrawTextureImplDesc(
        AssetImporterImpl_Texture.AssetInfo assetInfo, 
        AssetImporterImpl_Texture.AssetInfo left, 
        AssetImporterImpl_Texture.AssetInfo right)
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
    
    private void DrawFBXImpl(AssetImporterImpl_FBX fbxImpl)
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
    
    private void DrawSoundImpl(AssetImporterImpl_Sound soundImpl)
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
        foreach (var assetImporterGUI in _assetImporterGuis)
        {
            assetImporterGUI.TrySave();
        }
        
        Close();
    }
}
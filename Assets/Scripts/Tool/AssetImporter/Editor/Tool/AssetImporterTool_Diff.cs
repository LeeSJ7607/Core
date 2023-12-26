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
        public AssetImporter_TextureImpl TextureImpl { get; }
        public AssetImporter_FBXImpl FBXImpl { get; }
        public AssetImporter_SoundImpl SoundImpl { get; }

        public AssetInfo(
            AssetImporter_TextureImpl textureImpl = null,
            AssetImporter_FBXImpl fbxImpl = null,
            AssetImporter_SoundImpl soundImpl = null)
        {
            TextureImpl = textureImpl;
            FBXImpl = fbxImpl;
            SoundImpl = soundImpl;
        }
    }

    private AssetImporterPart _part;
    private AssetInfo _before, _after;
    private AssetType _curAssetType;
    private Vector2 _scrollPos;

    public static void Open(AssetImporterPart part, AssetImporter_TextureImpl before, AssetImporter_TextureImpl after)
    {
        var tool = GetWindow<AssetImporterTool_Diff>("Diff");
        tool.minSize = tool.maxSize = new Vector2(660, 800);
        tool._part = part;
        tool._before = new AssetInfo(before);
        tool._after = new AssetInfo(after);

        Sync(tool);
    }

    private static void Sync(AssetImporterTool_Diff diff)
    {
        var beforeAssetInfoMap = diff._before.TextureImpl.AssetInfoMap;
        var afterAssetInfoMap = diff._after.TextureImpl.AssetInfoMap;

        foreach (var (path, beforeTextures) in beforeAssetInfoMap)
        {
            for (var i = 0; i < beforeTextures.Count; i++)
            {
                beforeTextures[i].Changed = afterAssetInfoMap[path][i].Changed;
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

    private void DrawTextureImpl(AssetImporter_TextureImpl textureImpl)
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
                if (assetInfo.Changed == false)
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
        AssetImporter_TextureImpl.AssetInfo assetInfo, 
        AssetImporter_TextureImpl.AssetInfo left, 
        AssetImporter_TextureImpl.AssetInfo right)
    {
        const float keyWidth = 80;
        const float valueWidth = 170;
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
    
    private void DrawFBXImpl(AssetImporter_FBXImpl fbxImpl)
    {
        if (fbxImpl == null)
        {
            return;
        }
    }
    
    private void DrawSoundImpl(AssetImporter_SoundImpl soundImpl)
    {
        if (soundImpl == null)
        {
            return;
        }
    }
    
    private void DrawConfirmBtn()
    {
        if (GUILayout.Button("저장", GUIUtil.ButtonStyle(), GUILayout.Height(30)) == false)
        {
            return;
        }

        EditorUtility.DisplayDialog("알림", "변경된 에셋을 적용했습니다.", "확인");
        _part.TrySave();
        Close();
    }
}
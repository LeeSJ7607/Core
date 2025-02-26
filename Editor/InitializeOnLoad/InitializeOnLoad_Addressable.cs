using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;

[InitializeOnLoad]
internal static class InitializeOnLoad_Addressable
{
    static InitializeOnLoad_Addressable()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        settings.OnModification = (assetSettings, @event, obj) =>
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"Save Addressable.\nevent: {@event}, arg: {obj}");
        };
    }
}
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
            EditorApplication.ExecuteMenuItem("File/Save Project");
            Debug.Log($"Save Addressable.\nevent: {@event}, arg: {obj}");
        };
    }
}
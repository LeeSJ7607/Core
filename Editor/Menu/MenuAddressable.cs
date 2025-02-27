using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

internal sealed class MenuAddressable
{
    [MenuItem("Custom/Menu/Addressable/Clear Addressable All")]
    private static void ClearAddressableAll()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var entries = new List<AddressableAssetEntry>(settings.DefaultGroup.entries);
            
        foreach (var entry in entries)
        {
            settings.RemoveAssetEntry(entry.guid);
        }
    }
    
    [MenuItem("Custom/Menu/Addressable/Replace Addressable To FileName All")]
    private static void ReplaceAddressableToFileNameAll()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        foreach (var group in settings.groups)
        {
            if (group.name.Contains("Built In Data"))
            {
                continue;
            }
            
            foreach (var entry in group.entries)
            {
                entry.address = Path.GetFileNameWithoutExtension(entry.AssetPath);
            }
        }
    }
}
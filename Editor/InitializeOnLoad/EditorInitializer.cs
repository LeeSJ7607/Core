using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;

[InitializeOnLoad]
internal static class EditorInitializer
{
    static EditorInitializer()
    {
        AutoRenameHierarchyTree();
        OnModificationAddressable();
    }

    private static void AutoRenameHierarchyTree()
    {
        ObjectFactory.componentWasAdded += (component) =>
        {
            if (component is not HierarchyTree)
            {
                return;
            }

            component.gameObject.name = component.GetType().Name;
        };
    }
    
    private static void OnModificationAddressable()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        settings.OnModification = (_, modificationEvent, obj) =>
        {
            EditorApplication.ExecuteMenuItem("File/Save Project");
            Debug.Log($"OnModificationAddressable: {modificationEvent}, arg: {obj}");
        };
    }
}
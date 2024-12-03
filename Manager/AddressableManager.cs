using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

internal sealed class AddressableManager : Singleton<AddressableManager>
{
    private readonly SerializableDictionary<string, UnityEngine.Object> _resMap = new();
    
    public void Release()
    {
        _resMap.Clear();
    }

    public TObject Get<TObject>(string name) where TObject : UnityEngine.Object
    {
        if (_resMap.TryGetValue(name, out var obj))
        {
            return obj as TObject;
        }
        
        Debug.LogError($"{name} is Not Found Resource");
        return null;
    }

    public async UniTask LoadAssetsAsync(string label)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label, typeof(UnityEngine.Object));
        var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(locations, null);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"{label} is Handle Status Failed");
            return;
        }
        
        var result = handle.Result;
        foreach (var obj in result)
        {
            _resMap.TryAdd(obj.name, obj);
        }
        
        Addressables.Release(handle);
    }
}
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

internal sealed class AddressableManager : Singleton<AddressableManager>
{
    private readonly Dictionary<string, UnityEngine.Object> _resMap = new();
    
    public void Release()
    {
        _resMap.Clear();
    }

    public TObject Get<TObject>(string name = null) where TObject : UnityEngine.Object
    {
        var key = name ?? typeof(TObject).Name;
        
        if (_resMap.TryGetValue(key, out var obj))
        {
            return obj as TObject;
        }
        
        Debug.LogError($"{key} is Not Found Resource");
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
        
        //TODO: 바로 해제 하는 게 맞나? 어드레서블 프로파일러 확인 필요.
        Addressables.Release(handle);
    }
}
using System.Collections.Generic;
using UnityEngine;

internal sealed class UIContainer
{
    private readonly Dictionary<int, UIBase> _uiBaseMap = new();

    public void Release()
    {
        _uiBaseMap.Clear();
    }

    public T GetOrCreate<T>(Transform root) where T : UIBase
    {
        if (_uiBaseMap.TryGetValue(typeof(T).GetHashCode(), out var uiBase))
        {
            return uiBase as T;
        }

        return Create<T>(root);
    }

    private T Create<T>(Transform root) where T : UIBase
    {
        var fileName = GetFileName<T>();
        var res = AddressableManager.Instance.Get<GameObject>(fileName);

        if (UnityEngine.Object.Instantiate(res, root).TryGetComponent<T>(out var uiBase))
        {
            _uiBaseMap.Add(typeof(T).GetHashCode(), uiBase);
            return uiBase;
        }
        
        Debug.LogError($"Resource: {res}, UIBase Component Confirm Required");
        return null;
    }
    
    private string GetFileName<T>() where T : UIBase
    {
        var uiPathTable = AddressableManager.Instance.Get<UIPathTable>(nameof(UIPathTable));
        return uiPathTable.GetFileName(typeof(T).Name);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class UIContainer
{
    private readonly Dictionary<Type, UIBase> _uiBaseMap = new();

    public void Release()
    {
        _uiBaseMap.Clear();
    }

    public TBase GetOrCreate<TBase>(Transform root) where TBase : UIBase
    {
        if (_uiBaseMap.TryGetValue(typeof(TBase), out var uiBase))
        {
            return (TBase)uiBase;
        }

        return Create<TBase>(root);
    }

    private TBase Create<TBase>(Transform root) where TBase : UIBase
    {
        var fileName = GetFileName<TBase>();
        var res = AddressableManager.Instance.Get<GameObject>(fileName);
        var obj = UnityEngine.Object.Instantiate(res, root);

        if (obj.TryGetComponent<TBase>(out var uiBase))
        {
            _uiBaseMap.Add(typeof(TBase), uiBase);
            return uiBase;
        }
        
        Debug.LogError($"Resource: {res}, UIBase Component Confirm Required");
        UnityEngine.Object.Destroy(obj);
        return null;
    }
    
    private string GetFileName<TBase>() where TBase : UIBase
    {
        var uiPathSO = AddressableManager.Instance.Get<UIPathSO>(nameof(UIPathSO));
        return uiPathSO.GetFileName(typeof(TBase).Name);
    }
}
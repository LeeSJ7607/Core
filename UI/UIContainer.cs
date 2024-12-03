using System.Collections.Generic;
using UnityEngine;

internal sealed class UIContainer
{
    private readonly Dictionary<int, UIBase> _uiBaseMap = new();

    public void Release()
    {
        _uiBaseMap.Clear();
    }

    public TBase GetOrCreate<TBase>(Transform root) where TBase : UIBase
    {
        if (_uiBaseMap.TryGetValue(typeof(TBase).GetHashCode(), out var uiBase))
        {
            return uiBase as TBase;
        }

        return Create<TBase>(root);
    }

    private TBase Create<TBase>(Transform root) where TBase : UIBase
    {
        var fileName = GetFileName<TBase>();
        var res = AddressableManager.Instance.Get<GameObject>(fileName);

        if (UnityEngine.Object.Instantiate(res, root).TryGetComponent<TBase>(out var uiBase))
        {
            _uiBaseMap.Add(typeof(TBase).GetHashCode(), uiBase);
            return uiBase;
        }
        
        Debug.LogError($"Resource: {res}, UIBase Component Confirm Required");
        return null;
    }
    
    private string GetFileName<TBase>() where TBase : UIBase
    {
        var uiPathTable = AddressableManager.Instance.Get<UIPathTable>(nameof(UIPathTable));
        return uiPathTable.GetFileName(typeof(TBase).Name);
    }
}
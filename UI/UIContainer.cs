using System.Collections.Generic;
using UnityEngine;

public interface ISlotCreator
{
    TSlot Create<TSlot>(Transform root) where TSlot : UISlot;
}

internal sealed class UIContainer : ISlotCreator
{
    private readonly Dictionary<int, IReadOnlyUIBase> _uiBaseMap = new();

    public void Release()
    {
        _uiBaseMap.Clear();
    }
    
    TSlot ISlotCreator.Create<TSlot>(Transform root)
    {
        return GetOrCreate<TSlot>(root);
    }

    public TBase GetOrCreate<TBase>(Transform root) where TBase : IReadOnlyUIBase
    {
        if (_uiBaseMap.TryGetValue(typeof(TBase).GetHashCode(), out var uiBase))
        {
            return (TBase)uiBase;
        }

        return Create<TBase>(root);
    }

    private TBase Create<TBase>(Transform root) where TBase : IReadOnlyUIBase
    {
        var fileName = GetFileName<TBase>();
        var res = AddressableManager.Instance.Get<GameObject>(fileName);

        if (UnityEngine.Object.Instantiate(res, root).TryGetComponent<TBase>(out var uiBase))
        {
            uiBase.SetSlotCreator(this);
            _uiBaseMap.Add(typeof(TBase).GetHashCode(), uiBase);
            return uiBase;
        }
        
        Debug.LogError($"Resource: {res}, UIBase Component Confirm Required");
        return default;
    }
    
    private string GetFileName<TBase>() where TBase : IReadOnlyUIBase
    {
        var uiPathTable = AddressableManager.Instance.Get<UIPathTable>(nameof(UIPathTable));
        return uiPathTable.GetFileName(typeof(TBase).Name);
    }
}
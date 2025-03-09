using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ModelManager : Singleton<ModelManager>
{
    private readonly Dictionary<Type, IModel> _modelMap = new()
    {
        { typeof(CMUser), new CMUser() },
        { typeof(CMRanking), new CMRanking() },
    };

    public void Release()
    {
        foreach (var model in _modelMap.Values)
        {
            model.Release();
        }
    }
    
    public void Initialize()
    {
        RefreshModelMap();
        
        foreach (var model in _modelMap.Values)
        {
            model.Initialize();
        }
    }
    
    public void OnUpdate()
    {
        foreach (var model in _modelMap.Values)
        {
            model.Update();
        }
    }

    private void RefreshModelMap()
    {
        var loadedModels = CalcLoadModels();

        foreach (var (type, model) in loadedModels)
        {
            _modelMap[type] = model;
        }
    }
    
    private IReadOnlyDictionary<Type, IModel> CalcLoadModels()
    {
        var dic = new Dictionary<Type, IModel>();
        
        foreach (var (type, model) in _modelMap)
        {
            if (model.TryLoad(out var refModel))
            {
                dic.Add(type, refModel);
            }
        }

        return dic;
    }
    
    public void SaveAll()
    {
        foreach (var model in _modelMap.Values)
        {
            model.Save();
        }
    }

    public TModel Get<TModel>() where TModel : IModel
    {
        if (_modelMap.TryGetValue(typeof(TModel), out var model))
        {
            return (TModel)model;
        }

        Debug.LogError($"Not Found Model: {typeof(TModel)}");
        return default;
    }
}
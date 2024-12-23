using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ModelManager : Singleton<ModelManager>
{
    private readonly Dictionary<Type, IReadOnlyModel> _modelMap = new()
    {
        { typeof(CMUser), new CMUser() },
        { typeof(CMRanking), new CMRanking() },
    };
    
    public void SaveAll()
    {
        foreach (var model in _modelMap.Values)
        {
            model.Save();
        }
    }

    public void LoadAll()
    {
        var loadedModels = CalcLoadModels();

        foreach (var (type, model) in loadedModels)
        {
            _modelMap[type] = model;
        }
    }
    
    private IReadOnlyDictionary<Type, IReadOnlyModel> CalcLoadModels()
    {
        var dic = new Dictionary<Type, IReadOnlyModel>();
        
        foreach (var (type, model) in _modelMap)
        {
            if (model.TryLoad(out var refModel))
            {
                dic.Add(type, refModel);
            }
        }

        return dic;
    }

    public TModel Get<TModel>() where TModel : Model
    {
        if (_modelMap.TryGetValue(typeof(TModel), out var model))
        {
            return model as TModel;
        }

        Debug.LogError($"Not Found Model: {typeof(TModel)}");
        return null;
    }
}
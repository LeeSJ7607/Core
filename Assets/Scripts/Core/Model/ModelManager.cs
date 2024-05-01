using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ModelManager : Singleton<ModelManager>
{
    private static readonly string _savePath = $"{Application.persistentDataPath}/Model";
    private readonly Dictionary<Type, IModel> _modelMap = new()
    {
        { typeof(CMUser), new CMUser() },
        { typeof(SMUser), new SMUser() },
    };
    
    public void Release()
    {
        foreach (var (_, model) in _modelMap)
        {
            FileUtil.SaveAsJson(_savePath, model);
        }
    }

    public void Initialize()
    {
        SyncAll(LoadFileAll());
    }
    
    private void SyncAll(IReadOnlyDictionary<Type, IModel> models)
    {
        foreach (var (type, model) in models)
        {
            _modelMap[type] = model;
            
            if (_modelMap[type] is IClientModel clientModel)
            {
                clientModel.Sync();
            }
        }
    }

    private IReadOnlyDictionary<Type, IModel> LoadFileAll()
    {
        var dic = new Dictionary<Type, IModel>(_modelMap.Count);
        
        foreach (var (type, _) in _modelMap)
        {
            var filePath = $"{_savePath}/{type.Name}";
            var loadedModel = (IModel)FileUtil.LoadFromJson(filePath, type);
            
            if (loadedModel != null)
            {
                dic.Add(type, loadedModel);
            }
        }

        return dic;
    }

    public TModel Get<TModel>() where TModel : class, IModel
    {
        if (_modelMap.TryGetValue(typeof(TModel), out var model))
        {
            return model as TModel;
        }

        Debug.LogError($"Not Found Model: {typeof(TModel)}");
        return null;
    }
}
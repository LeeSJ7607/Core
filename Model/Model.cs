using UnityEngine;

internal interface IReadOnlyModel
{
    void Release();
    void Save();
    bool TryLoad(out IReadOnlyModel refModel);
}

public abstract class Model : IReadOnlyModel
{
    private static readonly string _savePath = $"{Application.persistentDataPath}/Model";
    private bool _canSave;

    protected abstract void OnRelease();
    void IReadOnlyModel.Release()
    {
        OnRelease();
    }
    
    void IReadOnlyModel.Save()
    {
        if (_canSave)
        {
            FileUtil.SaveAsJson(_savePath, this);
        }
    }

    bool IReadOnlyModel.TryLoad(out IReadOnlyModel refModel)
    {
        var type = GetType();
        var filePath = $"{_savePath}/{type.Name}";
        var model = (IReadOnlyModel)FileUtil.LoadFromJson(filePath, type);

        refModel = model;
        return model != null;
    }

    public void EnableSave()
    {
        _canSave = true;
    }
}
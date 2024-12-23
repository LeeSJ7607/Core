using UnityEngine;

internal interface IReadOnlyModel
{
    void Save();
    bool TryLoad(out IReadOnlyModel refModel);
}

internal abstract class Model : IReadOnlyModel
{
    private static readonly string _savePath = $"{Application.persistentDataPath}/Model";
    private bool _canSave;
    
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
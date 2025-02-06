using UnityEngine;

internal interface IReadOnlyModel
{
    void Release();
    void Initialize();
    void Update();
    void Save();
    bool TryLoad(out IReadOnlyModel refModel);
}

public abstract class Model : IReadOnlyModel
{
    private bool _canSave;
    
    protected abstract void OnRelease();
    
    void IReadOnlyModel.Release()
    {
        OnRelease();
    }

    void IReadOnlyModel.Initialize()
    {
        OnInitialize();
    }
    
    void IReadOnlyModel.Update()
    {
        OnUpdate();
    }
    
    void IReadOnlyModel.Save()
    {
        if (_canSave)
        {
            FileUtil.SaveAsJson(CalcSavePath(), this);
        }
    }

    bool IReadOnlyModel.TryLoad(out IReadOnlyModel refModel)
    {
        var type = GetType();
        var filePath = $"{CalcSavePath()}/{type.Name}";
        var model = (IReadOnlyModel)FileUtil.LoadFromJson(filePath, type);

        refModel = model;
        return model != null;
    }
    
    private string CalcSavePath()
    {
        var cmUser = ModelManager.Instance.Get<CMUser>();
        return $"{Application.persistentDataPath}{cmUser.UserId}/Model";
    }

    protected virtual void OnInitialize()
    {
        
    }
    
    protected virtual void OnUpdate()
    {
        
    }
    
    public void EnableSave()
    {
        _canSave = true;
    }
}
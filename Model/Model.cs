using UnityEngine;

public interface IModel
{
    void Release();
    void Initialize();
    void Update();
    void Save();
    bool TryLoad(out IModel refModel);
}

//TODO: 테이블이 새로운 로우가 추가되거나, 변경이 되었을 경우 기존에 저장한 로컬 테이블 데이터와 싱크를 맞춰봐야함
public abstract class Model : IModel
{
    private bool _canSave;
    
    protected abstract void OnRelease();
    
    void IModel.Release()
    {
        OnRelease();
    }

    void IModel.Initialize()
    {
        OnInitialize();
    }
    
    void IModel.Update()
    {
        OnUpdate();
    }
    
    void IModel.Save()
    {
        if (_canSave)
        {
            FileUtil.SaveAsJson(CalcSavePath(), this);
        }
    }

    bool IModel.TryLoad(out IModel refModel)
    {
        var type = GetType();
        var filePath = $"{CalcSavePath()}/{type.Name}";
        var model = (IModel)FileUtil.LoadFromJson(filePath, type);

        refModel = model;
        return model != null;
    }
    
    private string CalcSavePath()
    {
        var cmUser = ModelManager.GetModel<CMUser>();
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
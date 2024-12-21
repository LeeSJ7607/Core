using UnityEngine;

public abstract class UIBase : MonoBehaviour, IMVCView
{
    private IMVCController _mvcController;
    
    public void CreateMVCController(IMVCController mvcController)
    {
        _mvcController = mvcController;
        _mvcController.Initialize(this);
    }
    
    protected virtual void OnDestroy()
    {
        _mvcController?.Release();
    }
    
    protected virtual void Awake()
    {
        
    }
    
    public virtual void Show()
    {
        gameObject.Show();
    }
    
    public virtual void Hide()
    {
        gameObject.Hide();
    }
}
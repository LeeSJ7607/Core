using UnityEngine;

internal abstract class UIBase<TController> : MonoBehaviour, IMVCView 
    where TController : IMVCController, new()
{
    protected TController _mvcController;
    
    protected virtual void OnDestroy()
    {
        _mvcController.Release();
    }
    
    protected virtual void Awake()
    {
        _mvcController = new TController();
        _mvcController.Initialize(this);
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
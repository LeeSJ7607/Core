using R3;
using UnityEngine;

public abstract class UIBase : MonoBehaviour, IMVCView
{
    private IMVCController _mvcController;
    protected readonly CompositeDisposable _disposable = new();
    
    protected void SetMVCController(IMVCController mvcController)
    {
        _mvcController = mvcController;
        _mvcController.Initialize(this);
    }
    
    protected virtual void OnDestroy()
    {
        _disposable.Dispose();
        _mvcController?.Release();
        _mvcController = null;
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
        _disposable.Clear();
        gameObject.Hide();
    }
}
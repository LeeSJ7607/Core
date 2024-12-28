using R3;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected readonly CompositeDisposable _disposable = new();
    
    protected virtual void OnDestroy()
    {
        _disposable.Dispose();
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
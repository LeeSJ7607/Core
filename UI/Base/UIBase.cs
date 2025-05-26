using R3;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public bool IsShown { get; private set; }
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
        if (IsShown)
        {
            return;
        }
        
        gameObject.Show();
        IsShown = true;
    }
    
    public virtual void Hide()
    {
        if (!IsShown)
        {
            return;
        }
        
        _disposable.Clear();
        gameObject.Hide();
        IsShown = false;
    }
}
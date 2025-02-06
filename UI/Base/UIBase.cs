using R3;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected readonly CompositeDisposable _disposable = new();
    public bool ActiveSelf { get; private set; }
    
    protected virtual void OnDestroy()
    {
        _disposable.Dispose();
    }
    
    protected virtual void Awake()
    {
        
    }
    
    public virtual void Show()
    {
        if (ActiveSelf)
        {
            return;
        }
        
        gameObject.Show();
        ActiveSelf = true;
    }
    
    public virtual void Hide()
    {
        if (!ActiveSelf)
        {
            return;
        }
        
        _disposable.Clear();
        gameObject.Hide();
        ActiveSelf = false;
    }
}
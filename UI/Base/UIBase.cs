using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected virtual void OnDestroy()
    {
        
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
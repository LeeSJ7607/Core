using System.Collections.Generic;
using UnityEngine;

internal abstract class BaseCanvas
{
    private readonly UIContainer _uiContainer = new();
    private readonly Stack<UIPopup> _popups = new();
    private readonly Transform _root;
    
    protected BaseCanvas(Transform root)
    {
        _root = root;
    }
    
    public virtual void Initialize()
    {
        
    }
    
    public void OnTick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PopPopup();
        }
    }
    
    private void PopPopup()
    {
        if (_popups.IsNullOrEmpty())
        {
            //ShowPopup<UIPopup_System>();
            return;
        }

        var popup = _popups.Pop();
        popup.Hide();
    }
    
    protected T ShowPopup<T>() where T : UIPopup
    {
        var popup = _uiContainer.GetOrCreate<T>(_root);
        popup.Show();
        
        _popups.Push(popup);
        return popup;
    }
}
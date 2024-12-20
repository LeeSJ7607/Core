using System.Collections.Generic;
using UnityEngine;

internal abstract class BaseCanvas
{
    private readonly UIContainer _uiContainer = new();
    private readonly Stack<UIPopup> _backKeyPopups = new();
    private readonly Transform _root;
    
    protected BaseCanvas(Transform root)
    {
        _root = root;
    }
    
    public void Release()
    {
        _uiContainer.Release();
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
        if (_backKeyPopups.IsNullOrEmpty())
        {
            ShowSystemPopup();
            return;
        }

        var popup = _backKeyPopups.Pop();
        popup.Hide();
    }

    private void ShowSystemPopup()
    {
        var popup = ShowPopup<UIPopup_System>();
        popup.Set("게임을 종료하시겠습니까?");
    }
    
    protected TPopup ShowPopup<TPopup>() where TPopup : UIPopup
    {
        var popup = _uiContainer.GetOrCreate<TPopup>(_root);
        popup.Show();

        if (popup.CanBackKey)
        {
            _backKeyPopups.Push(popup);
        }

        return popup;
    }
}
using System.Collections.Generic;
using UnityEngine;

internal interface IReadOnlyBaseCanvas
{
    void Release();
    void Initialize();
    void OnTick();
}

internal abstract class BaseCanvas : IReadOnlyBaseCanvas
{
    private readonly UIContainer _uiContainer = new();
    private readonly Stack<UIPopup> _backKeyPopups = new();
    private readonly Transform _root;
    
    protected BaseCanvas(Transform root)
    {
        _root = root;
    }
    
    void IReadOnlyBaseCanvas.Release()
    {
        _uiContainer.Release();
    }

    void IReadOnlyBaseCanvas.Initialize()
    {
        OnInitialize();
    }
    
    protected virtual void OnInitialize()
    {
        
    }
    
    void IReadOnlyBaseCanvas.OnTick()
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
    
    protected TSlot ShowSlot<TSlot>() where TSlot : UISlot
    {
        var slot = _uiContainer.GetOrCreate<TSlot>(_root);
        slot.Show();

        return slot;
    }
}
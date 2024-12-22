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
    private readonly Stack<UIPopup> _popups = new();
    private readonly Transform _root;
    private readonly HandleBackButton _handleBackButton;
    
    protected BaseCanvas(Transform root)
    {
        _root = root;
        _handleBackButton = new HandleBackButton(_popups);
    }
    
    void IReadOnlyBaseCanvas.Release()
    {
        _uiContainer.Release();
    }

    protected abstract void OnInitialize();
    void IReadOnlyBaseCanvas.Initialize()
    {
        OnInitialize();
    }
    
    void IReadOnlyBaseCanvas.OnTick()
    {
        ProcessHandleBackButton();
    }

    private void ProcessHandleBackButton()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (!_handleBackButton.TryProcess())
        {
            ShowSystemPopup();
        }
    }
    
    private void ShowSystemPopup()
    {
        var popup = ShowPopup<UIPopup_System>();
        popup.Set("게임을 종료하시겠습니까?");
    }
    
    protected TPopup ShowPopup<TPopup>() where TPopup : UIPopup
    {
        var popup = ShowUI<TPopup>();
        if (popup.CanBackButton)
        {
            _popups.Push(popup);
        }

        return popup;
    }
    
    protected TSlot ShowSlot<TSlot>() where TSlot : UISlot
    {
        return ShowUI<TSlot>();
    }

    private T ShowUI<T>() where T : UIBase
    {
        var slot = _uiContainer.GetOrCreate<T>(_root);
        slot.Show();

        return slot;
    }
}
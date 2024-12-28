using System.Collections.Generic;
using UnityEngine;

internal sealed class UIManager : MonoSingleton<UIManager> 
{
    private readonly UIContainer _uiContainer = new();
    private readonly Stack<UIPopup> _popups = new();
    private HandleBackButton _handleBackButton;
    
    public void Release()
    {
        _uiContainer.Release();
        _popups.Clear();
    }

    public void Initialize()
    {
        _handleBackButton = new HandleBackButton(_popups);
    }
    
    private void Update()
    {
        ProcessHandleBackButton();
    }
    
    private void ProcessHandleBackButton()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        _handleBackButton.Execute();
    }
    
    public TPopup ShowPopup<TPopup>() where TPopup : UIPopup
    {
        var popup = ShowUI<TPopup>();
        if (popup.CanBackButton)
        {
            _popups.Push(popup);
        }

        return popup;
    }
    
    public TSlot ShowSlot<TSlot>() where TSlot : UISlot
    {
        return ShowUI<TSlot>();
    }

    private T ShowUI<T>() where T : UIBase
    {
        var slot = _uiContainer.GetOrCreate<T>(transform);
        slot.Show();

        return slot;
    }
}
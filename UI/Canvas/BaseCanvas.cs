using System.Collections.Generic;
using UnityEngine;

internal interface IReadOnlyBaseCanvas
{
    void Release();
    void Initialize();
    void OnTick();
}

internal abstract partial class BaseCanvas : IReadOnlyBaseCanvas
{
    private readonly UIContainer _uiContainer = new();
    private readonly Stack<UIPopup> _backButtonPopups = new();
    private readonly Transform _root;
    
    protected BaseCanvas(Transform root)
    {
        _root = root;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }
    
    protected TPopup ShowPopup<TPopup>() where TPopup : UIPopup
    {
        var popup = _uiContainer.GetOrCreate<TPopup>(_root);
        popup.Show();
        
        if (popup.CanBackButton)
        {
            _backButtonPopups.Push(popup);
        }

        return popup;
    }
}
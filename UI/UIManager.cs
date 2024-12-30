using System.Collections.Generic;

internal sealed class UIManager : MonoSingleton<UIManager> 
{
    private readonly UIContainer _uiContainer = new();
    private readonly Stack<UIPopup> _popups = new();
    private readonly HandleBackButton _handleBackButton = new();
    
    public void Release()
    {
        HidePopupAll();
        _popups.Clear();
        _uiContainer.Release();
    }

    protected override void Update()
    {
        base.Update();
        _handleBackButton.Execute(_popups);
    }
    
    private void HidePopupAll()
    {
        foreach (var popup in _popups)
        {
            popup.Hide();
        }
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
using System.Collections.Generic;

internal sealed class HandleBackButton
{
    private readonly Stack<UIPopup> _popups;

    public HandleBackButton(Stack<UIPopup> popups)
    {
        _popups = popups;
    }
    
    public bool TryProcess()
    {
        if (_popups.IsNullOrEmpty())
        {
            return false;
        }
        
        PopPopup();
        return true;
    }
    
    private void PopPopup()
    {
        var popup = _popups.Pop();
        popup.Hide();
    }
}
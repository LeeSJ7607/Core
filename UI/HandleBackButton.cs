using System.Collections.Generic;

internal sealed class HandleBackButton
{
    private readonly Stack<UIPopup> _popups;

    public HandleBackButton(Stack<UIPopup> popups)
    {
        _popups = popups;
    }
    
    public void Execute()
    {
        if (_popups.IsNullOrEmpty())
        {
            var popup = UIManager.Instance.ShowPopup<UIPopup_System>();
            popup.Set("게임을 종료하시겠습니까?");
            return;
        }
        
        PopPopup();
    }
    
    private void PopPopup()
    {
        var popup = _popups.Pop();
        popup.Hide();
    }
}
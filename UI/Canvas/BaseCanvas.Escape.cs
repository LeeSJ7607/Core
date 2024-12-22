internal abstract partial class BaseCanvas
{
    private void HandleBackButton()
    {
        PopPopup();
    }
    
    private void PopPopup()
    {
        if (_backButtonPopups.IsNullOrEmpty())
        {
            ShowSystemPopup();
            return;
        }

        var popup = _backButtonPopups.Pop();
        popup.Hide();
    }

    private void ShowSystemPopup()
    {
        var popup = ShowPopup<UIPopup_System>();
        popup.Set("게임을 종료하시겠습니까?");
    }
}
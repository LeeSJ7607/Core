internal abstract partial class BaseCanvas
{
    private void HandleBackButton()
    {
        PopPopup();
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
}
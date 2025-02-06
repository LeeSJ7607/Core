internal sealed class Command_Login_Patch : ICommand
{
    private UIPopup_Patch _popup;
    
    public void Execute()
    {
        _popup = UIManager.Instance.ShowPopup<UIPopup_Patch>();
    }

    public bool IsFinished()
    {
        return !_popup.ActiveSelf;
    }
}
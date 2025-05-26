internal sealed class Command_Login_AccountLogin : ICommand
{
    //private UIPopup_AccountLogin _popup;
    
    public void Execute()
    {
        //_popup = UIManager.Instance.ShowPopup<UIPopup_AccountLogin>();
    }

    public bool IsFinished()
    {
        //return !_popup.ActiveSelf;
        return true;
    }
}
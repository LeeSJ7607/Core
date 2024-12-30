using System.Collections.Generic;
using UnityEngine;

internal sealed class HandleBackButton
{
    public void Execute(Stack<UIPopup> popups)
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (!CanBackButton())
        {
            return;
        }
        
        if (popups.IsNullOrEmpty())
        {
            ShowSystemPopup();
            return;
        }
        
        PopPopup(popups);
    }
    
    private void ShowSystemPopup()
    {
        var popup = UIManager.Instance.ShowPopup<UIPopup_System>();
        popup.Set("게임을 종료하시겠습니까?");
    }
    
    private void PopPopup(Stack<UIPopup> popups)
    {
        var popup = popups.Pop();
        popup.Hide();
    }

    private bool CanBackButton()
    {
        return true;
    }
}
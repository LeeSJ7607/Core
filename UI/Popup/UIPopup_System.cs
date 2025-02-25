using UnityEngine;

internal sealed class UIPopup_System : UIPopup
{
    [SerializeField] private TextMeshProUGUIEx _txtTitle, _txtDesc;
    
    public void Set(int msgId, int titleId = 0)
    {
        _txtTitle.SetLocalizedText(titleId);
        _txtDesc.SetLocalizedText(msgId);
    }
}
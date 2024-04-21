using UnityEngine;

internal sealed class LoginCanvas : BaseCanvas
{
    public LoginCanvas(Transform root) : base(root)
    {
        
    }
    
    public override void Initialize()
    {
        base.Initialize();

        ShowPopup<UIPopup_System>();
    }
}
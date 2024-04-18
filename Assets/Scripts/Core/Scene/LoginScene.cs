internal sealed class LoginScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        
        _baseCanvas = new LoginCanvas(transform);
        _baseCanvas.Initialize();
    }
}
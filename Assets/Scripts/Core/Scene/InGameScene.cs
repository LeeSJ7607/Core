internal sealed class InGameScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        
        _baseCanvas = new InGameCanvas(transform);
        _baseCanvas.Initialize();
    }
}
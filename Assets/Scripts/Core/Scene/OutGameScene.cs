internal sealed class OutGameScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();
        
        _baseCanvas = new OutGameCanvas(transform);
        _baseCanvas.Initialize();
    }
}
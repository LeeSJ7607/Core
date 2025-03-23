internal sealed class Hero : Unit
{
    private HeroController _heroController;
    
    protected override void OnInitialize()
    {
        _heroController = new HeroController(this);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _heroController.OnUpdate();
    }
}
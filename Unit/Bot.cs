internal sealed class Bot : Unit
{
    private readonly UnitAIController _unitAIController = new();

    protected override void OnInitialize()
    {
        _unitAIController.Initialize(this, _unitController.Units);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _unitAIController.OnUpdate();
    }
}
internal sealed class UnitAIController
{
    private readonly IBTComposite _btRoot = new BTSequence();
    private readonly BTBoard _board;
    private bool _isActive;
    
    public UnitAIController(Unit unit)
    {
        _board = new BTBoard(unit);
    }

    public void Initialize()
    {
        //TODO: 툴에서 제작한 BT를 가져와 설정을 해야함.
        _btRoot.AddNode(new BTAction_Patrol())
               .AddNode(new BTAction_Chase())
               .AddNode(new BTAction_Attack());
        
        _isActive = true;
    }
    
    public void OnUpdate()
    {
        if (!_isActive)
        {
            return;
        }

        if (_btRoot.Update(_board) == EBTStatus.Success)
        {
            _isActive = false;
        }
    }
}
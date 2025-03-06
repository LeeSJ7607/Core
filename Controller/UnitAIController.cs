internal sealed class UnitAIController
{
    private readonly IBTComposite _btRoot;
    private readonly BTBoard _board;
    private bool _isActive = true;
    
    public UnitAIController(Unit unit)
    {
        _board = new BTBoard(unit);
        
        //TODO: 툴에서 제작한 BT를 가져와 설정을 해야함.
        _btRoot = new BTSequence();
        _btRoot.AddNode(new BTAction_Roaming())
               .AddNode(new BTAction_Chase())
               .AddNode(new BTAction_Attack());
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
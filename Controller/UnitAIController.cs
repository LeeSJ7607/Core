using System.Collections.Generic;

internal sealed class UnitAIController
{
    private readonly IBTComposite _btRoot = new BTSequence();
    private BlackBoard _board;
    private bool _isActive;

    public void Release()
    {
        _board.Release();
    }

    public void Initialize(IReadOnlyUnit owner, IEnumerable<IReadOnlyUnit> units)
    {
        _board ??= new BlackBoard(owner, units);
        
        //TODO: 툴에서 제작한 BT를 가져와 설정을 해야함.
        _btRoot.AddNode(new BTAction_Patrol())
               .AddNode(new BTAction_Chase())
               .AddNode(new BTAction_Attack());
        
        _isActive = true;
    }
    
    public void OnUpdate()
    {
        if (_isActive)
        {
            _btRoot.Update(_board);
        }
    }
}
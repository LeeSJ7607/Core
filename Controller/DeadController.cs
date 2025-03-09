using R3;

public sealed class DeadController
{
    private readonly Unit _unit;
    private readonly AnimatorController _animatorController;
    private readonly CompositeDisposable _disposable = new();
    
    public DeadController(Unit unit)
    {
        _unit = unit;
        _animatorController = unit.AnimatorController;
    }
    
    public void Release()
    {
        _disposable.Dispose();
    }
    
    public void Initialize()
    {
        _animatorController.OnAnimStateExit
                           .Subscribe(OnAnimStateExit)
                           .AddTo(_disposable);
    }

    private void OnAnimStateExit(EAnimState animState)
    {
        if (animState != EAnimState.Die)
        {
            return;
        }
        
        // var fx = Object.Instantiate(ResourceManager.Instance.Get<GameObject>("Fx_Unit_Die"));
        // fx.transform.position = _owner.transform.position;
        //
        // Object.Destroy(_owner.gameObject);
    }
}
using R3;

public sealed class DeadController
{
    private readonly Unit _owner;
    private readonly AnimatorController _animatorController;
    private readonly CompositeDisposable _disposable = new();
    
    public DeadController(Unit owner)
    {
        _owner = owner;
        _animatorController = owner.AnimatorController;
        
        owner.OnRelease
             .Subscribe(_ => _disposable.Dispose())
             .AddTo(_disposable);
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
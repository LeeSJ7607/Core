using R3;

public sealed class DeadController
{
    private readonly IReadOnlyUnit _owner;
    private readonly IAnimatorController _animatorController;
    private readonly CompositeDisposable _disposable = new();
    
    public DeadController(IReadOnlyUnit owner)
    {
        _owner = owner;
        _animatorController = owner.AnimatorController;
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

        ShowDieEffect();
    }

    private void ShowDieEffect()
    {
        // var fx = Object.Instantiate(ResourceManager.Instance.Get<GameObject>("Fx_Unit_Die"));
        // fx.transform.position = _owner.Pos;
        //
        // Object.Destroy(_owner.gameObject);
    }
}
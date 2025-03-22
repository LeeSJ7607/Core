using R3;
using UnityEngine;

public sealed class AttackController
{
    private IDefender _target;
    private readonly IAttacker _owner;
    private readonly CompositeDisposable _disposable = new();
    
    public AttackController(IAttacker owner)
    {
        _owner = owner;
                
        owner.AnimatorController.OnAnimStateExit
             .Subscribe(OnAnimStateExit)
             .AddTo(_disposable);

        var animationEventReceiver = owner.Tm.AddComponent<AnimationEventReceiver>(); 
        animationEventReceiver.OnAttack
                              .Subscribe(DoAttack)
                              .AddTo(_disposable);
    }

    public void Release()
    {
        _disposable.Dispose();
    }

    public bool IsTargetInRange(Vector3 targetPos)
    {
        return Vector3.Distance(_owner.Tm.position, targetPos) < _owner.UnitTable.Atk_Range;
    }
    
    public void Attack(IDefender target)
    {
        _target = target;
        LookAtTarget(target);

        if (_owner.IsAttackable)
        {
            _owner.IsAttackable = false;
            _owner.AnimatorController.SetState(EAnimState.Attack);
        }
    }
    
    private void LookAtTarget(IDefender target)
    {
        var dir = (target.Pos - _owner.Tm.position).normalized;
        _owner.Tm.rotation = Quaternion.LookRotation(dir);
    }
    
    private void DoAttack(AnimationEvent animationEvent)
    {
        if (_target != null)
        {
            _target.Hit(_owner.Damage);
        }
    }
    
    private void OnAnimStateExit(EAnimState animState)
    {
        if (animState == EAnimState.Attack)
        {
            _owner.IsAttackable = true;
        }
    }
}
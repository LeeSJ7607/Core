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
        owner.OnRelease
             .Subscribe(_ => _disposable.Dispose())
             .AddTo(_disposable);

        var animationEventReceiver = owner.Tm.AddComponent<AnimationEventReceiver>(); 
        animationEventReceiver.OnAttack
                              .Subscribe(DoAttack)
                              .AddTo(_disposable);
    }

    public bool IsTargetInRange(Vector3 targetPos)
    {
        return Vector3.Distance(_owner.Tm.position, targetPos) < _owner.UnitTable.Atk_Range;
    }
    
    //TODO: 매프레임마다 SetState 을 해도 되려는지.
    public void Attack(IDefender target)
    {
        _target = target;
        LookAtTarget(target);
        _owner.AnimatorController.SetState(EAnimState.Attack);
    }
    
    private void DoAttack(AnimationEvent animationEvent)
    {
        if (_target != null)
        {
            _target.Hit(_owner.Damage);
        }
    }

    private void LookAtTarget(IDefender target)
    {
        var dir = (target.Pos - _owner.Tm.position).normalized;
        _owner.Tm.rotation = Quaternion.LookRotation(dir);
    }
}
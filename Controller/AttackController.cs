using R3;
using UnityEngine;

public sealed class AttackController
{
    private IDefender _target;
    private readonly IAttacker _owner;
    private readonly Transform _ownerTm;
    private readonly AnimatorController _animatorController;
    private readonly CompositeDisposable _disposable = new();
    
    public AttackController(IReadOnlyUnit owner)
    {
        _owner = owner;
        _ownerTm = owner.Tm;
        _animatorController = owner.AnimatorController;
        
        owner.OnRelease
             .Subscribe(_ => _disposable.Dispose())
             .AddTo(_disposable);

        var animationEventReceiver = _ownerTm.AddComponent<AnimationEventReceiver>(); 
        animationEventReceiver.OnAttack
                              .Subscribe(DoAttack)
                              .AddTo(_disposable);
    }

    public bool IsTargetInRange(Vector3 targetPos)
    {
        return Vector3.Distance(_ownerTm.position, targetPos) < 2f; //TODO: 2f 는 공격 사정거리 (테이블 필요)
    }
    
    //TODO: 매프레임마다 SetState 을 해도 되려는지.
    public void Attack(IDefender target)
    {
        _target = target;
        LookAtTarget(target);
        _animatorController.SetState(EAnimState.Attack);
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
        var dir = (target.Tm.position - _ownerTm.position).normalized;
        _ownerTm.rotation = Quaternion.LookRotation(dir);
    }
}
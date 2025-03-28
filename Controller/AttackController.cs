using R3;
using UnityEngine;

public sealed class AttackController
{
    private IDefender _target;
    private readonly IAttacker _owner;
    private readonly Transform _ownerTm;
    private readonly float _attackRange;
    private readonly IAnimatorController _animatorController;
    private readonly CompositeDisposable _disposable = new();
    
    public AttackController(IAttacker owner)
    {
        _owner = owner;

        var readOnlyUnit = (IReadOnlyUnit)owner;
        _ownerTm = readOnlyUnit.Tm;
        _attackRange = readOnlyUnit.UnitTable.Atk_Range;
        _animatorController = readOnlyUnit.AnimatorController;
        _animatorController.OnAnimStateExit
                           .Subscribe(OnAnimStateExit)
                           .AddTo(_disposable);

        var animationEventReceiver = _ownerTm.AddComponent<AnimationEventReceiver>(); 
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
        var sqr = (targetPos - _ownerTm.position).sqrMagnitude;
        return sqr < _attackRange * _attackRange;
    }
    
    public void Attack(IDefender target)
    {
        _target = target;
        LookAtTarget(target);

        if (_owner.IsAttackable)
        {
            _animatorController.SetState(EAnimState.Attack);
            _owner.IsAttackable = false;
        }
    }
    
    private void LookAtTarget(IDefender target)
    {
        var dir = (target.Pos - _ownerTm.position).normalized;
        _ownerTm.rotation = Quaternion.Slerp(_ownerTm.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
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
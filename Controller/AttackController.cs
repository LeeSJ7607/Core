using UnityEngine;

public sealed class AttackController
{
    private readonly IAttacker _owner;
    private readonly Transform _ownerTm;
    private readonly AnimatorController _animatorController;
    
    public AttackController(Unit owner)
    {
        _owner = owner;
        _ownerTm = owner.transform;
        _animatorController = owner.AnimatorController;
    }
    
    public bool IsTargetInRange(Vector3 targetPos)
    {
        return Vector3.Distance(_ownerTm.transform.position, targetPos) < 2f; //TODO: 2f 는 공격 사정거리 (테이블 필요)
    }
    
    //TODO: 매프레임마다 SetState 을 해도 되려는지.
    public void Attack(IDefender target)
    {
        LookAtTarget(target);

        if (_owner.IsAttackable)
        {
            _owner.IsAttackable = false;
            target.Hit(_owner.Damage);
        }
        else
        {
            _animatorController.SetState(EAnimState.Attack);
        }
    }

    private void LookAtTarget(IDefender target)
    {
        var dir = (target.Tm.position - _ownerTm.position).normalized;
        _ownerTm.rotation = Quaternion.LookRotation(dir);
    }
}
using UnityEngine;

public interface IDefender
{
    Vector3 Pos { get; }
    void Hit(long damage);
}

public abstract partial class Unit
{
    Vector3 IDefender.Pos => transform.position;
    
    void IDefender.Hit(long damage)
    {
        _stat[eStat.HP] -= damage;
        _unitUI.SetHPAndDamage(_stat, damage);
        AnimatorController.SetState(IsDead ? eAnimState.Die : eAnimState.Hit);
        
        if (IsDead)
        {
            _unitController.RemoveUnit(this);
        }
    }
}
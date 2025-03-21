using UnityEngine;

public interface IDefender
{
    Vector3 Pos { get; }
    void Hit(int damage);
}

public abstract partial class Unit
{
    Vector3 IDefender.Pos => transform.position;
    
    void IDefender.Hit(int damage)
    {
        _stat[EStat.HP] -= damage;
        _unitUI.SetHPAndDamage(_stat, damage);
        AnimatorController.SetState(IsDead ? EAnimState.Die : EAnimState.Hit);
        
        if (IsDead)
        {
            _unitController.RemoveUnit(this);
        }
    }
}
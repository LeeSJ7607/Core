using UnityEngine;

public interface IDefender
{
    Vector3 Pos { get; }
    void Hit(long damage);
    void AddBuff(Buff buff);
    void RemoveBuffFlags(eBuffEffect buffEffectFlags);
}

public abstract partial class Unit
{
    Vector3 IDefender.Pos => transform.position;
    private eBuffEffect _curBuffEffectFlags;
    
    void IDefender.Hit(long damage)
    {
        _stat[eStat.HP] -= damage;
        _unitUI.SetHPAndDamage(_stat, damage);
        AnimatorController.SetState(IsDead ? eAnimState.Die : eAnimState.Hit);
        
        if (IsDead)
        {
            _unitContainer.RemoveUnit(this);
        }
    }
    
    void IDefender.AddBuff(Buff buff)
    {
        _curBuffEffectFlags |= buff.BuffTable.BuffEffectType;
        _buffController.AddBuff(buff);
    }

    void IDefender.RemoveBuffFlags(eBuffEffect buffEffectFlags)
    {
        _curBuffEffectFlags &= ~buffEffectFlags;
    }
}
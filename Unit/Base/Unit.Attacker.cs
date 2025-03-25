using UnityEngine;

public interface IAttacker
{
    bool IsAttackable { get; set; }
    int Damage { get; }
    UnitTable.Row UnitTable { get; }
    Transform Tm { get; }
    IAnimatorController AnimatorController { get; }
}

public abstract partial class Unit
{
    bool IAttacker.IsAttackable { get; set; } = true;
    int IAttacker.Damage => 100;
    UnitTable.Row IAttacker.UnitTable => DataAccessor.GetTable<UnitTable>().GetRow(_unitId);
    Transform IAttacker.Tm => transform;
    IAnimatorController IAttacker.AnimatorController => _animatorController;
}
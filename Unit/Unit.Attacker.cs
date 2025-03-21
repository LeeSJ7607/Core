using R3;
using UnityEngine;

public interface IAttacker
{
    int Damage { get; }
    UnitTable.Row UnitTable { get; }
    Transform Tm { get; }
    IAnimatorController AnimatorController { get; }
    Observable<R3.Unit> OnRelease { get; }
}

public abstract partial class Unit
{
    int IAttacker.Damage => 100;
    UnitTable.Row IAttacker.UnitTable => DataAccessor.GetTable<UnitTable>().GetRow(_unitId);
    Transform IAttacker.Tm => transform;
    IAnimatorController IAttacker.AnimatorController => _animatorController;
    Observable<R3.Unit> IAttacker.OnRelease => _onRelease;
}
using System.Collections.Generic;
using UnityEngine;

public sealed class BlackBoard
{
    private readonly IReadOnlyUnit _owner;
    public Vector3 OwnerPos => _owner.Tm.position;
    public MoveController MoveController { get; }
    public TargetController TargetController { get; }
    public AttackController AttackController { get; }
    public IEnumerable<IReadOnlyUnit> Units { get; }
    public IReadOnlyUnit Target => TargetController.Target;

    public BlackBoard(IReadOnlyUnit owner, IEnumerable<IReadOnlyUnit> units)
    {
        _owner = owner;
        Units = units;

        if (owner is Unit unit)
        {
            MoveController = unit.MoveController;
            TargetController = unit.TargetController;
            AttackController = unit.AttackController;
        }
    }

    public void Release()
    {
        AttackController.Release();
    }
}
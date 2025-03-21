using System.Collections.Generic;
using UnityEngine;

public sealed class BlackBoard
{
    public MoveController MoveController { get; }
    public TargetController TargetController { get; }
    public AttackController AttackController { get; }
    public Vector3 OwnerPos { get; }
    public IEnumerable<IReadOnlyUnit> Units { get; }
    public IReadOnlyUnit Target => TargetController.Target;

    public BlackBoard(IReadOnlyUnit owner, IEnumerable<IReadOnlyUnit> units)
    {
        MoveController = new MoveController(owner);
        TargetController = new TargetController(owner);
        AttackController = new AttackController(owner as IAttacker);
        OwnerPos = owner.Tm.position;
        Units = units;
    }
}
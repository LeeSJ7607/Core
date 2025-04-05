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
        MoveController = new MoveController(owner);
        TargetController = new TargetController(owner);
        AttackController = new AttackController((IAttacker)owner);
        Units = units;
    }

    public void Release()
    {
        AttackController.Release();
    }
}
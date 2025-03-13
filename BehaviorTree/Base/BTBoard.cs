using UnityEngine;

public sealed class BTBoard
{
    public MoveController MoveController { get; }
    public TargetController TargetController { get; }
    public AttackController AttackController { get; }
    public Vector3 OwnerPos { get; }
    public Unit Target => TargetController.Target;

    public BTBoard(Unit unit)
    {
        MoveController = new MoveController(unit);
        TargetController = new TargetController(unit);
        AttackController = new AttackController(unit);
        OwnerPos = unit.transform.position;
    }
}
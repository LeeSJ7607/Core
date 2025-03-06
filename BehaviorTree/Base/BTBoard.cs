using UnityEngine;

public sealed class BTBoard
{
    public Vector3 OwnerPos { get; }
    public AnimatorController AnimatorController { get; }
    public MoveController MoveController { get; }
    public TargetController TargetController { get; }
    public AttackController AttackController { get; }

    public BTBoard(Unit unit)
    {
        OwnerPos = unit.transform.position;
        AnimatorController = unit.AnimatorController;
        MoveController = new MoveController(unit);
        TargetController = new TargetController(unit);
        AttackController = new AttackController(unit);
    }
}
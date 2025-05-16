using System.Collections.Generic;
using UnityEngine;

public sealed class BlackBoard
{
    private readonly IReadOnlyUnit _owner;
    public Vector3 OwnerPos => _owner.Tm.position;
    public MoveController MoveController { get; }
    public TargetController TargetController { get; }
    public AttackController AttackController { get; }
    public SkillController SkillController { get; }
    public IEnumerable<IReadOnlyUnit> Units { get; }
    public IReadOnlyUnit Target => TargetController.Target;

    public BlackBoard(IReadOnlyUnit owner, IEnumerable<IReadOnlyUnit> units)
    {
        if (owner is not Unit unit)
        {
            Debug.LogError($"[{nameof(BlackBoard)}] Owner is not Unit. Actual: {owner.GetType().Name}");
            return;
        }

        _owner = owner;
        Units = units;
        MoveController = unit.MoveController;
        TargetController = unit.TargetController;
        AttackController = unit.AttackController;
        SkillController = unit.SkillController;
    }
}
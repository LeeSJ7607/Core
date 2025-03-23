using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SeekByFOVEnemy : ISeeker
{
    public IReadOnlyList<IReadOnlyUnit> Seek(IEnumerable<IReadOnlyUnit> units, IReadOnlyUnit owner)
    {
        IReadOnlyUnit target = null;
        var enemies = units.GetEnemies(owner.FactionType);

        foreach (var unit in enemies)
        {
            var unitTable = unit.UnitTable;

            var dir = unit.Tm.position - owner.Tm.position;
            if (dir.magnitude > unitTable.FOV_Radius)
            {
                continue;
            }
            
            var fovAngle = Mathf.Cos(unitTable.FOV_Angle * Mathf.Rad2Deg);
            var dot = Vector3.Dot(owner.Tm.forward.normalized, dir.normalized);
            if (dot < fovAngle)
            {
                continue;
            }

            target = unit;
            break;
        }

        return target != null ? new [] { target } : Array.Empty<IReadOnlyUnit>();
    }
}
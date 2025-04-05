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
            if (unit.Tm.position.SqrDistance(owner.Tm.position) > unitTable.FOV_Radius.Sqr())
            {
                continue;
            }
            
            var dir = unit.Tm.position - owner.Tm.position;
            var dot = Vector3.Dot(owner.Tm.forward.normalized, dir.normalized);
            var fovAngle = Mathf.Cos(unitTable.FOV_Angle * Mathf.Deg2Rad);
            
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
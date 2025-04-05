using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SeekByNearestEnemy : ISeeker
{
    public IReadOnlyList<IReadOnlyUnit> Seek(IEnumerable<IReadOnlyUnit> units, IReadOnlyUnit owner)
    {
        IReadOnlyUnit target = null;
        var nearestDistance = float.MaxValue;
        var enemies = units.GetEnemies(owner.FactionType);
        
        foreach (var unit in enemies)
        {
            var sqrMagnitude = unit.Tm.position.SqrDistance(owner.Tm.position);
            if (sqrMagnitude > unit.UnitTable.FOV_Radius.Sqr())
            {
                continue;
            }
         
            if (sqrMagnitude >= nearestDistance)
            {
                continue;
            }

            nearestDistance = sqrMagnitude;
            target = unit;
        }

        return target != null ? new [] { target } : Array.Empty<IReadOnlyUnit>();
    }
}
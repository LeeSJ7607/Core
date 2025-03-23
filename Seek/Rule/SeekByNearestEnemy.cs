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
            var dis = Vector3.Distance(owner.Tm.position, unit.Tm.position);
            if (dis >= nearestDistance)
            {
                continue;
            }

            nearestDistance = dis;
            target = unit;
        }

        return target != null ? new [] { target } : Array.Empty<IReadOnlyUnit>();
    }
}
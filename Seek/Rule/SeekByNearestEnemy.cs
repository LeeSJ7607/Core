using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class SeekByNearestEnemy : ISeeker
{
    public IReadOnlyList<IReadOnlyUnit> Seek(IEnumerable<IReadOnlyUnit> units, IReadOnlyUnit owner)
    {
        IReadOnlyUnit target = null;
        var nearestDistance = float.MaxValue;

        var filteredUnits = units.FilterByFaction(EFaction.Enemy);
        foreach (var unit in filteredUnits)
        {
            if (unit.FactionType == owner.FactionType)
            {
                continue;
            }
            
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
using System;
using System.Collections.Generic;

public sealed class SeekByRandom : ISeeker
{
    private readonly Random _rand = new();

    IReadOnlyList<Unit> ISeeker.Seek(Unit owner)
    {
        //TODO: 개발 필요.
        return null;
        // var units = SpawnedUnitContainer.Instance.GetAllFaction(owner_.FactionType);
        // var count = units.Count;
        //
        // if (count == 0)
        // {
        //     return null;
        // }
        //
        // for (var i = 0; i < count; i++)
        // {
        //     var r = _rand.Next(count - i) + i;
        //     (units[i], units[r]) = (units[r], units[i]);
        // }
        //
        // return new [] { units[0] };
    }
}
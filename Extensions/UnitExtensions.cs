using System.Collections.Generic;

public static class UnitExtensions
{
    public static IEnumerable<IReadOnlyUnit> FilterByFaction(this IEnumerable<IReadOnlyUnit> source, EFaction faction)
    {
        var result = new List<IReadOnlyUnit>();

        foreach (var unit in source)
        {
            if (unit.FactionType == faction)
            {
                result.Add(unit);
            }
        }

        return result;
    }
}
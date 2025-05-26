using System.Collections.Generic;

public static class UnitExtensions
{
    public static bool IsDead(this IReadOnlyUnit source)
    {
        return source == null || source.IsDead;
    }
    
    public static IEnumerable<IReadOnlyUnit> GetEnemies(this IEnumerable<IReadOnlyUnit> source, eFaction faction)
    {
        var result = new List<IReadOnlyUnit>();

        foreach (var unit in source)
        {
            if (unit.FactionType != faction)
            {
                result.Add(unit);
            }
        }

        return result;
    }
}
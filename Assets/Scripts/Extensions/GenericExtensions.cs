using System.Collections.Generic;

public static class GenericExtensions
{
    public static bool Empty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }
}
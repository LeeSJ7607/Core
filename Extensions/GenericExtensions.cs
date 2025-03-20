using System;
using System.Linq;
using System.Collections.Generic;

public static class GenericExtensions
{
    public static bool IsNullOrEmpty<T>(this T[] source)
    {
        return source.IsNull() || source.Length == 0;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source.IsNull() || !source.Any();
    }

    public static void Foreach<T>(this IReadOnlyList<T> source, Action<T> act)
    {
        if (source.IsNullOrEmpty())
        {
            return;
        }

        foreach (var element in source)
        {
            act.Invoke(element);
        }
    }
}
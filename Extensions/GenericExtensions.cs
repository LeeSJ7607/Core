using System;
using System.Linq;
using System.Collections.Generic;

public static class GenericExtensions
{
    public static bool IsNullOrEmpty<T>(this T[] param)
    {
        return param.IsNull() || param.Length == 0;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> param)
    {
        return param.IsNull() || !param.Any();
    }

    public static void Foreach<T>(this IReadOnlyList<T> list, Action<T> act)
    {
        if (list.IsNullOrEmpty())
        {
            return;
        }

        foreach (var element in list)
        {
            act.Invoke(element);
        }
    }
}
using System.Collections.Generic;
using System.Linq;

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
}
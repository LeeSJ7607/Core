using System;
using System.Linq;

public static class EnumUtil
{
    public static string[] GetValidEnumNames<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
                   .Cast<TEnum>()
                   .Where(e => Convert.ToInt32(e) > -1)
                   .Select(e => e.ToString()).ToArray();
    }
}
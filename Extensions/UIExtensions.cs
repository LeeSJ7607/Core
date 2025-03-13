public static class UIExtensions
{
    public static string ToStringMoney(this int value)
    {
        return value.ToString("N0");
    }
}
public static class UIExtensions
{
    public static string ToStringMoney(this int source)
    {
        return source.ToString("N0");
    }
}
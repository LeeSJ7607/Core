public static class UIExtensions
{
    public static string ToStringMoney(this int source)
    {
        return source.ToString("N0");
    }
    
    public static string ToStringMoney(this long source)
    {
        return source.ToString("N0");
    }
}
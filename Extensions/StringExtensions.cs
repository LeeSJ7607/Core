public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string source)
    {
        return string.IsNullOrEmpty(source);
    }
    
    public static bool IsNullOrWhiteSpace(this string source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    public static bool IsEquals(this string source, string target)
    {
        if (source == null)
        {
            return false;
        }
        
        return source.Equals(target);
    }
}
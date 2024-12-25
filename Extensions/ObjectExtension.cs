public static class ObjectExtension
{
    public static bool IsNull(this object obj) => obj is null;
    public static bool IsNotNull(this object obj) => obj is null == false;
}
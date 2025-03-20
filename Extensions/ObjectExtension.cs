public static class ObjectExtension
{
    public static bool IsNull(this object source) => source is null;
    public static bool IsNotNull(this object source) => source is null == false;
}
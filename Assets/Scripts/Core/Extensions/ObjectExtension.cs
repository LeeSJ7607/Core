public static class ObjectExtension
{
    public static bool IsNull(this object this_) => this_ is null;
    public static bool IsNotNull(this object this_) => this_ is null == false;
}
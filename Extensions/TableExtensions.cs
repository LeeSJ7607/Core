public static class TableExtensions
{
    public static T Get<T>(this Stat stat) where T : IBaseTable
    {
        return TableManager.Instance.Get<T>();
    }
}
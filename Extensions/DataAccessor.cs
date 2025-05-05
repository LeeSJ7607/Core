public static class DataAccessor
{
    public static T GetModel<T>() where T : IModel
    {
        return ModelManager.Instance.Get<T>();
    }

    public static T GetTable<T>() where T : IBaseTable
    {
        return TableManager.Instance.Get<T>();
    }
    
    public static bool IsDead(this IReadOnlyUnit source)
    {
        return source == null || source.IsDead;
    }
}
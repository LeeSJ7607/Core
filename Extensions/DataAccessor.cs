public static class DataAccessor
{
    public static T GetModel<T>() where T : IModel
    {
        return ModelManager.Instance.Get<T>();
    }

    public static T GetTable<T>() where T : ITable
    {
        return TableManager.Instance.Get<T>();
    }
    
    public static bool IsDead(this IReadOnlyUnit unit)
    {
        return unit == null || unit.IsDead;
    }
}
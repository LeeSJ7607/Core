internal static class ModelExtensions
{
    public static T Get<T>(this UIBase uiBase) where T : Model
    {
        return ModelManager.Instance.Get<T>();
    }
}
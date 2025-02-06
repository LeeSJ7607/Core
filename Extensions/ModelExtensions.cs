internal static class ModelExtensions
{
    public static T GetData<T>(this UIBase uiBase) where T : Model
    {
        return ModelManager.Instance.Get<T>();
    }
}
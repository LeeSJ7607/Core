using UnityEngine.Purchasing;

internal sealed class IosInAppImpl : IInAppImpl
{
    void IInAppImpl.Initialize(ConfigurationBuilder builder)
    {
        AndroidInAppImpl.AddProudct(builder, AppleAppStore.Name, null);
    }
}
using UnityEngine.Purchasing;

internal sealed class PcInAppImpl : IInAppImpl
{
    void IInAppImpl.Initialize(ConfigurationBuilder builder)
    {
        AndroidInAppImpl.AddProudct(builder, AppleAppStore.Name, null);   
    }
}
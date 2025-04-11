using UnityEngine.Purchasing;

internal sealed class iOSIAPHandler : IIAPHandler
{
    void IIAPHandler.Initialize(ConfigurationBuilder builder)
    {
        AndroidIAPHandler.AddProudct(builder, GooglePlay.Name, null);
    }
}
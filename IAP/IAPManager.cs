using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

internal interface IInAppPlatform
{
    
}

internal sealed class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
{
    private IInAppPlatform _iapPlatform;

    public void Initialize()
    {
#if UNITY_ANDROID
        _iapPlatform = new AndroidInAppImpl();
#elif UNITY_IOS
        _iapPlatform = new iOSInAppImpl();
#else
        _iapPlatform = new PcInAppImpl();
#endif
    }
    
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) => throw new System.NotImplementedException();

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        throw new System.NotImplementedException();
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        throw new System.NotImplementedException();
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new System.NotImplementedException();
    }
}
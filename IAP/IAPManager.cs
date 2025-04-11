using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

internal interface IIAPHandler
{
    void Initialize(ConfigurationBuilder builder);
}

internal sealed class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
{
    private IIAPHandler _iapHandler;

    public async UniTaskVoid Initialize()
    {
        try 
        { 
            var option = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(option);
            
            _iapHandler = CreateIAPHandler();
            _iapHandler.Initialize(ConfigurationBuilder.Instance(StandardPurchasingModule.Instance()));
        }
        catch (Exception e)
        {
            Debug.LogError($"Unity Services Initialization failed: {e.Message}");
        }
    }

    private IIAPHandler CreateIAPHandler()
    {
#if UNITY_ANDROID
        return new AndroidIAPHandler();
#elif UNITY_IOS
        return new iOSIAPHandler();
#else
        return new PCIAPHandler();
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
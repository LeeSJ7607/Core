using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

internal interface IInAppImpl
{
    void Initialize(ConfigurationBuilder builder);
}

internal sealed class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
{
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;
    private IInAppImpl _inAppImpl;

    public async UniTaskVoid Initialize()
    {
        try 
        { 
            await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName("production"));
            
            _inAppImpl = CreateIAPHandler();
            _inAppImpl.Initialize(ConfigurationBuilder.Instance(StandardPurchasingModule.Instance()));
        }
        catch (Exception e)
        {
            Debug.LogError($"Unity Services Initialization failed: {e.Message}");
        }
    }

    private IInAppImpl CreateIAPHandler()
    {
        switch (Application.platform)
        {
        case RuntimePlatform.Android: return new AndroidInAppImpl();
        case RuntimePlatform.IPhonePlayer: return new IosInAppImpl();
        default: return new PcInAppImpl();
        }
    }
    
    public void OnInitialized(IStoreController storeController, IExtensionProvider extensionProvider)
    {
        _storeController = storeController;
        _extensionProvider = extensionProvider;
        RestoreProcess();
    }

    private void RestoreProcess()
    {
        foreach (var product in _storeController.products.all)
        {
            if (product.definition.type != ProductType.Consumable)
            {
                continue;
            }

            if (!product.hasReceipt)
            {
                continue;
            }
            
            //TODO: 복구를 한 상품이라면.
            {
                //continue;
            }
            
            //TODO: 상품 지급, 인앱 로그.
            _storeController.ConfirmPendingPurchase(product);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP Initialization failed: {error}");
    }
    
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Initialization failed: {error}, msg: {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        return default;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        
    }
}
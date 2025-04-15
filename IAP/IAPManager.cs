using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.Extension;

internal interface IInAppImpl
{
    void Initialize(IExtensionProvider extensionProvider);
    void RestoreTransactions(Action<bool, string> act);
}

//TODO: 인앱 결제는 마켓에서 구매 완료 후, 게임 보상이 지급 되지 않았을 경우에 대한 예외처리가 많이 필요함.
//TODO: 예를 들면, 마켓에서 구매 완료 후 게임을 강제 종료해서 게임 보상 지급이 되지 않았을 경우와 마켓에서 구매 완료 후 네트워크가 끊겨서 게임 보상 지급이 되지 않았을 경우. 
internal sealed class IAPManager : Singleton<IAPManager>, IDetailedStoreListener
{
    private IStoreController _storeController;
    private IExtensionProvider _extensionProvider;
    private IInAppImpl _inAppImpl;
    private Action<bool> _onPurchaseResult;

    public void Initialize()
    {
        try 
        { 
            InitializePurchasing().Forget();
        }
        catch (Exception e)
        {
            Debug.LogError($"Unity Services Initialization failed: {e.Message}");
        }
    }

    private async UniTaskVoid InitializePurchasing()
    {
        await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName("production"));
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        var storeName = Application.platform == RuntimePlatform.Android ? GooglePlay.Name : AppleAppStore.Name;
        var productIds = new List<string>();
        
        foreach (var productId in productIds)
        {
            if (productId.IsNullOrWhiteSpace())
            {
                Debug.LogError("Product ID is null or empty.");
                continue;
            }

            builder.AddProduct(productId, ProductType.Consumable, new IDs()
            {
                { productId, storeName }
            });
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController storeController, IExtensionProvider extensionProvider)
    {
        _storeController = storeController;
        _extensionProvider = extensionProvider;
        CreateIAPHandler().Initialize(extensionProvider);
        RestoreProcess();
    }
    
    private IInAppImpl CreateIAPHandler()
    {
        _inAppImpl = Application.platform == RuntimePlatform.Android 
            ? new AndroidInAppImpl()
            : new IosInAppImpl();

        return _inAppImpl;
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

            if (!IsValidReceipt(product.receipt, out var receipts))
            {
                _storeController.ConfirmPendingPurchase(product);
                continue;
            }
            
            var receipt = Array.Find(receipts, item => item.productID == product.definition.id);
            if (receipt == null)
            {
                _storeController.ConfirmPendingPurchase(product);
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

    public void Purchase(string productId, Action<bool> onResult)
    {
        var product = _storeController.products.WithID(productId);
        if (product == null)
        {
            Debug.LogError($"Product {productId} Purchase Failed. Please check your product id.");
            return;
        }
        
        _onPurchaseResult = onResult;
        _storeController.InitiatePurchase(product);
    }
    
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var productId = purchaseEvent.purchasedProduct.definition.id;
        var productReceipt = purchaseEvent.purchasedProduct.receipt;
        
        if (productReceipt.IsNullOrWhiteSpace())
        {
            Debug.LogError($"Receipt is null or empty. ProductId: {productId}");
            return PurchaseProcessingResult.Complete;
        }

        if (!IsValidReceipt(productReceipt, out var refReceipts))
        {
            _onPurchaseResult?.Invoke(false);
            return PurchaseProcessingResult.Complete;
        }

        var receipt = Array.Find(refReceipts, receipt => receipt.productID == productId);
        if (receipt == null)
        {
            _onPurchaseResult?.Invoke(false);
            return PurchaseProcessingResult.Complete;
        }
        
        //TODO: 인앱 로그.
        {
            //receipt
        }
        
        _onPurchaseResult?.Invoke(true);
        _storeController.ConfirmPendingPurchase(purchaseEvent.purchasedProduct);
        return PurchaseProcessingResult.Complete;
    }

    private bool IsValidReceipt(string receipt, out IPurchaseReceipt[] receipts)
    {
        receipts = null;
        
        try
        {
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            receipts = validator.Validate(receipt);
        }
        catch (IAPSecurityException e)
        {
            Debug.LogError($"Invalid receipt: {e.Message}");
        }

        return receipts != null;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        switch (failureDescription.reason)
        {
        case PurchaseFailureReason.UserCancelled:
            {
                //TODO: 구매 취소.
            }
            break;

        case PurchaseFailureReason.DuplicateTransaction:
            {
                //TODO: 중복 구매.
            }
            break;

        default:
            {
                Debug.LogError($"Purchase failed: {product.definition.id}, reason: {failureDescription.message}");
            }
            break;
        }
    }
    
    public void RestoreTransactions()
    {
        _inAppImpl.RestoreTransactions((success, error) =>
        {
            if (success)
            {
                RestoreProcess();
            }
            else
            {
                Debug.LogError($"Restore Transactions failed: {error}");
            }
        });
    }

    public float GetPrice(string productId)
    {
        var product = _storeController.products.WithID(productId);
        if (product == null)
        {
            Debug.LogError($"ProductId : {productId} is not found. Please check your product id.");
            return 0f;
        }

        return (float)product.metadata.localizedPrice;
    }

    public string GetPriceStr(string productId)
    {
        var product = _storeController.products.WithID(productId);
        if (product == null)
        {
            Debug.LogError($"ProductId : {productId} is not found. Please check your product id.");
            return string.Empty;
        }

        return product.metadata.localizedPriceString;
    }
    
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP Initialization failed: {error}, msg: {message}");
    }
    
#region Obsolete
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        
    }
#endregion
}
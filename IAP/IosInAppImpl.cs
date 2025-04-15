using System;
using UnityEngine;
using UnityEngine.Purchasing;

internal sealed class IosInAppImpl : IInAppImpl
{
    private IAppleExtensions _appleExtensions;
    
    void IInAppImpl.Initialize(IExtensionProvider extensionProvider)
    {
        _appleExtensions = extensionProvider.GetExtension<IAppleExtensions>();
        _appleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
    }

    private void OnDeferred(Product product)
    {
        //TODO: 보류 중인 상품이 있다고 유저한테 알려줘야 함. (상품 지급은 ProcessPurchase 함수가 불리므로 알아서 처리됨)
    }

    void IInAppImpl.RestoreTransactions(Action<bool, string> act)
    {
        _appleExtensions.RefreshAppReceipt(success =>
        {
            _appleExtensions.RestoreTransactions(act);
        }, error =>
        {
           Debug.LogError($"Failed to refresh app receipt: {error}"); 
        });
    }
}
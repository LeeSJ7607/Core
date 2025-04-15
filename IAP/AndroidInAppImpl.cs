using System;
using UnityEngine.Purchasing;

internal sealed class AndroidInAppImpl : IInAppImpl
{
    private IGooglePlayStoreExtensions _googlePlayStoreExtensions;
    
    void IInAppImpl.Initialize(IExtensionProvider extensionProvider)
    {
        _googlePlayStoreExtensions = extensionProvider.GetExtension<IGooglePlayStoreExtensions>();
    }

    void IInAppImpl.RestoreTransactions(Action<bool, string> act)
    {
        _googlePlayStoreExtensions.RestoreTransactions(act);
    }
}
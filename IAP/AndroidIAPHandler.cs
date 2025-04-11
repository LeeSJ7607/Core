using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine;

internal sealed class AndroidIAPHandler : IIAPHandler
{
    void IIAPHandler.Initialize(ConfigurationBuilder builder)
    {
        AddProudct(builder, GooglePlay.Name, null);
    }

    public static void AddProudct(ConfigurationBuilder builder, string storeName, IReadOnlyList<string> productIds)
    {
        foreach (var productId in productIds)
        {
            if (productId.IsNullOrWhiteSpace())
            {
                Debug.LogError("Product ID is null or empty.");
                continue;
            }

            builder.AddProduct(productId, ProductType.Consumable, new IDs()
            {
                productId, storeName
            });
        }

        UnityPurchasing.Initialize(IAPManager.Instance, builder);
    }
}
using Unity.VisualScripting;
using UnityEngine;

internal sealed class ShopView : IMVCView
{
    public void SetId(int id)
    {
        Debug.Log(id.ToString());
    }
}
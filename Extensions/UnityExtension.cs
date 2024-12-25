using UnityEngine;

public static class UnityExtension
{
    public static void Show(this GameObject obj)
    {
        if (obj.IsNull() || obj.activeSelf)
        {
            return;
        }
        
        obj.SetActive(true);
    }
    
    public static void Hide(this GameObject obj)
    {
        if (obj.IsNull() || !obj.activeSelf)
        {
            return;
        }
        
        obj.SetActive(false);
    }
}
using UnityEngine;

public static class UnityExtension
{
    public static void Show(this GameObject param)
    {
        if (param.IsNull() || param.activeSelf)
        {
            return;
        }
        
        param.SetActive(true);
    }
    
    public static void Hide(this GameObject param)
    {
        if (param.IsNull() || !param.activeSelf)
        {
            return;
        }
        
        param.SetActive(false);
    }
}
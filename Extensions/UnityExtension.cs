using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public static void AddListener(this Button btn, UnityAction act)
    {
        if (btn.IsNull())
        {
            return;
        }
        
        btn.onClick.AddListener(act);
    }

    public static void RemoveAllListeners(this Button btn)
    {
        if (btn.IsNull())
        {
            return;
        }
        
        btn.onClick.RemoveAllListeners();
    }
}
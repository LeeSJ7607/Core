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
    
    public static void Show(this Transform tm)
    {
        if (tm.IsNull())
        {
            return;
        }
        
        tm.gameObject.Show();
    }
    
    public static void Hide(this Transform tm)
    {
        if (tm.IsNull())
        {
            return;
        }
        
        tm.gameObject.Hide();
    }
    
    public static void Show(this MonoBehaviour mono)
    {
        if (mono.IsNull())
        {
            return;
        }
        
        mono.gameObject.Show();
    }
    
    public static void Hide(this MonoBehaviour mono)
    {
        if (mono.IsNull())
        {
            return;
        }
        
        mono.gameObject.Hide();
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
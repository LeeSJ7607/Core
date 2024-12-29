using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

    public static void SetPositionAndRotation(this GameObject obj, Transform tm)
    {
        if (obj.IsNull() || obj.transform.IsNull() || tm.IsNull())
        {
            return;
        }
        
        obj.transform.SetPositionAndRotation(tm.position, tm.rotation);
    }
    
    public static void SetPositionAndRotation(this GameObject obj, Vector3 pos, Quaternion rot)
    {
        if (obj.IsNull() || obj.transform.IsNull())
        {
            return;
        }
        
        obj.transform.SetPositionAndRotation(pos, rot);
    }

    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        var component = obj.GetComponent<T>();
        return component != null ? component : obj.AddComponent<T>();
    }
    
    public static T AddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.AddComponent<T>();
    }
}
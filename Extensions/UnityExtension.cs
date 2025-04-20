using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class UnityExtension
{
    public static void SetActive(this MonoBehaviour source, bool active)
    {
        if (source.IsNull())
        {
            return;
        }

        if (source.gameObject.activeSelf == active)
        {
            return;
        }
        
        source.gameObject.SetActive(active);
    }
    
    public static void Show(this GameObject source)
    {
        if (source.IsNull() || source.activeSelf)
        {
            return;
        }
        
        source.SetActive(true);
    }
    
    public static void Hide(this GameObject source)
    {
        if (source.IsNull() || !source.activeSelf)
        {
            return;
        }
        
        source.SetActive(false);
    }
    
    public static void Show(this Transform source)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.gameObject.Show();
    }
    
    public static void Hide(this Transform source)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.gameObject.Hide();
    }
    
    public static void Show(this MonoBehaviour source)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.gameObject.Show();
    }
    
    public static void Hide(this MonoBehaviour source)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.gameObject.Hide();
    }
    
    public static void SetParent(this GameObject source, GameObject root)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.transform.SetParent(root.transform);
    }
    
    public static void SetParent(this GameObject source, Transform root)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.transform.SetParent(root);
    }

    public static void AddListener(this Button source, UnityAction act)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.onClick.AddListener(act);
    }

    public static void RemoveAllListeners(this Button source)
    {
        if (source.IsNull())
        {
            return;
        }
        
        source.onClick.RemoveAllListeners();
    }

    public static void SetPositionAndRotation(this GameObject source, Transform tm)
    {
        if (source.IsNull() || source.transform.IsNull() || tm.IsNull())
        {
            return;
        }
        
        source.transform.SetPositionAndRotation(tm.position, tm.rotation);
    }
    
    public static void SetPositionAndRotation(this GameObject source, Vector3 pos, Quaternion rot)
    {
        if (source.IsNull() || source.transform.IsNull())
        {
            return;
        }
        
        source.transform.SetPositionAndRotation(pos, rot);
    }

    public static T GetOrAddComponent<T>(this GameObject source) where T : Component
    {
        var component = source.GetComponent<T>();
        return component != null ? component : source.AddComponent<T>();
    }
    
    public static T AddComponent<T>(this Component source) where T : Component
    {
        return source.gameObject.AddComponent<T>();
    }

    public static bool TryGetChild(this MonoBehaviour source, out Transform refTm, int idx = 0)
    {
        refTm = null;
        
        var tm = source.transform;
        if (tm.childCount == idx)
        {
            return false;
        }

        refTm = tm.GetChild(idx);
        return true;
    }

    public static void DestroyImmediate(this GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        
        Object.DestroyImmediate(obj);
    }
    
    public static void Destroy(this GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        
        Object.Destroy(obj);
    }
}
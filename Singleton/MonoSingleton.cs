using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance.IsNotNull())
            {
                return _instance;
            }

            _instance = CreateInstance();
            DontDestroyOnLoad(_instance);

            return _instance;
        }
    }

    private static T CreateInstance()
    {
        var findObjectOfType = FindAnyObjectByType<T>();
        if (findObjectOfType.IsNotNull())
        {
            return findObjectOfType;
        }
        
        var name = typeof(T).ToString();
        var obj = GameObject.Find(name);
        if (obj.IsNull())
        {
            obj = new GameObject(name);
        }
        
        return obj.AddComponent<T>();
    }

    protected virtual void Update()
    {
        
    }
}
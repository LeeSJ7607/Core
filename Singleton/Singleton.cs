using System;

internal class Singleton<T> where T : class
{
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            if (_instance.IsNull())
            {
                _instance = Activator.CreateInstance<T>();
            }
            
            return _instance;
        }
    }
}
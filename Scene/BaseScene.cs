using Cysharp.Threading.Tasks;
using UnityEngine;

internal abstract class BaseScene : MonoBehaviour
{
    protected readonly SceneLoader _sceneLoader = new();
    
    protected virtual void Awake()
    {
        
    }
    
    protected virtual async UniTask Start()
    {
        UIManager.Instance.Initialize();
    }
    
    protected virtual void Update()
    {
        
    }
}
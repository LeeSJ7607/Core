using Cysharp.Threading.Tasks;
using UnityEngine;

internal abstract class BaseScene : MonoBehaviour
{
    protected virtual void Awake()
    {
        
    }
    
    protected virtual async UniTask Start()
    {
        await AddressableManager.Instance.LoadAssetsAsync(SceneLoader.Instance.CurSceneType.ToString());
        UIManager.Instance.Initialize();
    }
    
    protected virtual void Update()
    {
        
    }
}
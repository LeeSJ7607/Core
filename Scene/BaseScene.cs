using Cysharp.Threading.Tasks;
using UnityEngine;

internal abstract class BaseScene : MonoBehaviour
{
    private IReadOnlyBaseCanvas _baseCanvas;
  
    private void OnDestroy()
    {
        _baseCanvas.Release();
    }
    
    protected virtual void Awake()
    {
        switch (SceneLoader.Instance.CurSceneType)
        {
        case EScene.Login: _baseCanvas = new LoginCanvas(transform); break;
        case EScene.OutGame: _baseCanvas = new OutGameCanvas(transform); break;
        case EScene.InGame: _baseCanvas = new InGameCanvas(transform); break;
        case EScene.Loading: _baseCanvas = new LoadingCanvas(transform); break;
        }
    }
    
    protected virtual async UniTask Start()
    {
        await AddressableManager.Instance.LoadAssetsAsync(SceneLoader.Instance.CurSceneType.ToString());
        _baseCanvas.Initialize();
    }
    
    private void Update()
    {
        _baseCanvas.OnTick();
    }

    private void OnApplicationQuit()
    {
        ModelManager.Instance.SaveAll();
    }
}
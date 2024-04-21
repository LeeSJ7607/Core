using Cysharp.Threading.Tasks;
using UnityEngine;

internal abstract class BaseScene : MonoBehaviour
{
    private BaseCanvas _baseCanvas;
    
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
        await AddressableManager.Instance.LoadAssetsAsync(nameof(LoginScene));
        _baseCanvas.Initialize();
    }
    
    private void OnDestroy()
    {
        _baseCanvas.Release();
    }
    
    private void Update()
    {
        _baseCanvas.OnTick();
    }
}
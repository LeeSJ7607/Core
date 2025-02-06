using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

internal sealed class LoadingScene : BaseScene
{
    protected override async UniTask Start()
    {
        await base.Start(); 
        await ReleaseAll();
        
        await AddressableManager.Instance.LoadAssetsAsync(_sceneLoader.CurSceneType.ToString());
        _sceneLoader.LoadNextScene();
    }

    private async UniTask ReleaseAll()
    {
        GC.Collect();
        await Resources.UnloadUnusedAssets();
        AddressableManager.Instance.Release();
        UIManager.Instance.Release();
        TableManager.Instance.Release();
    }
}
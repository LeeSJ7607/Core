using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

internal sealed class LoadingScene : BaseScene
{
    protected override async UniTask Start()
    {
        await base.Start(); 
        await ReleaseAll();
        SceneLoader.Instance.LoadNextScene();
    }

    private async UniTask ReleaseAll()
    {
        GC.Collect();
        await Resources.UnloadUnusedAssets();
        AddressableManager.Instance.Release();
    }
}
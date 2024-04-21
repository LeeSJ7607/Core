using Cysharp.Threading.Tasks;
using UnityEngine;

internal abstract class BaseScene : MonoBehaviour
{
    protected BaseCanvas _baseCanvas;
    
    protected virtual void Awake()
    {
        
    }
    
    protected virtual async UniTask Start()
    {
        await UniTask.CompletedTask;
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
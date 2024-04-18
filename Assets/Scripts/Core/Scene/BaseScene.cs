using UnityEngine;

internal abstract class BaseScene : MonoBehaviour
{
    protected BaseCanvas _baseCanvas;
    
    protected virtual void Awake()
    {
        
    }
    
    private void Update()
    {
        _baseCanvas.OnTick();
    }
}
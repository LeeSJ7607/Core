using System;
using UnityEngine;

internal abstract class BaseScene : MonoBehaviour
{
    private BaseCanvas _baseCanvas;
    
    protected virtual void Awake()
    {
        _baseCanvas = _baseCanvas switch
        {
            LoginCanvas => new LoginCanvas(transform),
            LoadingCanvas => new LoadingCanvas(transform),
            OutGameCanvas => new OutGameCanvas(transform),
            InGameCanvas => new InGameCanvas(transform),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private void Update()
    {
        _baseCanvas.OnTick();
    }
}
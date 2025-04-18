﻿using Cysharp.Threading.Tasks;
using UnityEngine;

internal abstract class Scene : MonoBehaviour
{
    protected readonly SceneLoader _sceneLoader = new();
    
    protected virtual void Awake()
    {
        
    }
    
    protected virtual async UniTask Start()
    {
        await UniTask.CompletedTask;
    }
    
    protected virtual void Update()
    {
        
    }
}
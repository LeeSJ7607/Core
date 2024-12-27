using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UniTaskExtensions
{
    public static UniTask UniTaskDelay(this MonoBehaviour mono, float second, CancellationTokenSource cancellationTokenSource = null)
    {
        var token = cancellationTokenSource?.Token ?? mono.GetCancellationTokenOnDestroy();
        return UniTask.Delay(TimeSpan.FromSeconds(second), cancellationToken: token);
    }
    
    public static UniTask UniTaskDelay(this Model model, float second, CancellationTokenSource cancellationTokenSource = null)
    {
        return UniTask.Delay(TimeSpan.FromSeconds(second), cancellationToken: cancellationTokenSource?.Token ?? default);
    }
}
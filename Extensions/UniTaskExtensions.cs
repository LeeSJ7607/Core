using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class UniTaskExtensions
{
    public static UniTask UniTaskDelay(this MonoBehaviour source, float second, CancellationTokenSource cancellationTokenSource = null)
    {
        var token = cancellationTokenSource?.Token ?? source.GetCancellationTokenOnDestroy();
        return UniTask.Delay(TimeSpan.FromSeconds(second), cancellationToken: token);
    }
    
    public static UniTask UniTaskDelay(this Model source, float second, CancellationTokenSource cancellationTokenSource = null)
    {
        return UniTask.Delay(TimeSpan.FromSeconds(second), cancellationToken: cancellationTokenSource?.Token ?? default);
    }
}
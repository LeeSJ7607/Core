using System;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

internal sealed class Sender
{
    public sealed class Request
    {
        public Type ResponseType { get; private set; }
        public UnityWebRequest WebRequest { get; private set; }
        public Request(Type responseType, UnityWebRequest webRequest)
        {
            ResponseType = responseType;
            WebRequest = webRequest;
        }

        public bool HasSameData(string url)
        {
            return WebRequest.url.Equals(url);
        }
    }
    
    private readonly Session _session;
    private readonly Queue<Request> _requests = new();
    
    public Sender(Session session)
    {
        _session = session;
        OnTick().Forget();
    }

    public void Release()
    {
        _requests.Clear();
    }

    private async UniTaskVoid OnTick()
    {
        while (_session.IsValidSession)
        {
            await UniTask.NextFrame();
            
            if (!_requests.TryDequeue(out var request))
            {
                continue;
            }
            
            var responseResult = await SendWebRequest(request.WebRequest);
            _session.Receiver.ReceiveProcess(responseResult, request);
        }
    }

    public void Push<TResponse>(IRequest request) where TResponse : IResponse
    {
        if (HasSameRequest(request))
        {
            return;
        }

        _requests.Enqueue(new Request(typeof(TResponse), CreateUnityWebRequest(request)));
    }

    private bool HasSameRequest(IRequest request)
    {
        return _requests.Any(_ => _.HasSameData(request.Url));
    }

    private UnityWebRequest CreateUnityWebRequest(IRequest request)
    {
        var body = _session.Serializer.Serialize(request);
        var isGetMethod = body.IsNullOrEmpty();
        
        const string address = "http://localhost:9001/";
        var url = $"{address}{request.Url}";
        var httpMethod = isGetMethod ? UnityWebRequest.kHttpVerbGET : UnityWebRequest.kHttpVerbPOST;
        
        var webRequest = new UnityWebRequest(url, httpMethod);
        webRequest.timeout = 3000;
        webRequest.SetRequestHeader("Content-Type", "application/x-memorypack");

        if (!isGetMethod)
        {
            webRequest.uploadHandler = new UploadHandlerRaw(body);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
        }

        return webRequest;
    }

    private async UniTask<EResponseResult> SendWebRequest(UnityWebRequest webRequest)
    {
        var remainedRetryCount = 3;
        while (remainedRetryCount > 0)
        {
            try
            {
                await webRequest.SendWebRequest();
                if (IsValidRequest(webRequest))
                {
                    return EResponseResult.Success;
                }

                remainedRetryCount = await DecrementRetryWithDelay(remainedRetryCount, webRequest.url);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                remainedRetryCount = await DecrementRetryWithDelay(remainedRetryCount, webRequest.url);
            }
        }
        
        return EResponseResult.Shutdown;
    }
    
    private async UniTask<int> DecrementRetryWithDelay(int remainedRetryCount, string webRequestUrl)
    {
        const float RETRY_DELAY_TIME_SECOND = 2f;

        if (remainedRetryCount > 1)
        {
            Debug.Log($"Network Retrying.. webRequestUrl: {webRequestUrl}, remainedRetryCount: {remainedRetryCount}");
            await UniTask.Delay(TimeSpan.FromSeconds(RETRY_DELAY_TIME_SECOND));
        }
        
        return --remainedRetryCount;
    }

    private bool IsValidRequest(UnityWebRequest webRequest)
    {
        if (webRequest.result == UnityWebRequest.Result.ConnectionError 
         || webRequest.result == UnityWebRequest.Result.ProtocolError 
         || webRequest.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.LogError(webRequest.error);
            return false;
        }

        return true;
    }
}
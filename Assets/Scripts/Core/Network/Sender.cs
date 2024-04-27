using System;
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
    }
    
    private readonly Session _session;
    private readonly Queue<Request> _requests = new();
    
    public Sender(Session session)
    {
        _session = session;
        OnTick().Forget();
    }

    private async UniTaskVoid OnTick()
    {
        var request = _requests.Dequeue();
        var responseResult = await SendWebRequest(request.WebRequest);
        _session.Receiver.ReceiveProcess(request, responseResult);
    }

    public void Push<TResponse>(IRequest request) where TResponse : IResponse
    {
        _requests.Enqueue(new Request(typeof(TResponse), CreateUnityWebRequest(request)));
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
        try
        {
            await webRequest.SendWebRequest();

            if (IsValidRequest(webRequest))
            {
                return EResponseResult.Success;
            }

            //TODO: 재시도.
            return EResponseResult.Retry;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return EResponseResult.Shutdown;
        }
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
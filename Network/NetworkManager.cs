using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

internal sealed class NetworkManager : Singleton<NetworkManager>
{
    private interface IMessage
    {
        void OnResponse(object message);
    }
    
    private sealed class Response<TResponse> : IMessage where TResponse : class, IResponse
    {
        public UniTask<TResponse> Task => _tcs.Task;
        private readonly UniTaskCompletionSource<TResponse> _tcs = new();
        public void OnResponse(object message)
        {
            _tcs.TrySetResult(message as TResponse);
        }
    }
    
    private readonly Session _session = new();
    private readonly Dictionary<Type, IMessage> _responseMap = new();
    
    public UniTask<TResponse> SendAsync<TResponse>(IRequest request) where TResponse : class, IResponse
    {
        _session.Send<TResponse>(request);

        return CreateResponse<TResponse>().Task;
    }

    public void Receive(Type responseType, object body)
    {
        if (_responseMap.Remove(responseType, out var old))
        {
            old.OnResponse(body);
        }
    }

    private Response<TResponse> CreateResponse<TResponse>() where TResponse : class, IResponse
    {
        var type = typeof(TResponse);

        if (_responseMap.Remove(type, out var old))
        {
            old.OnResponse(null);
        }
        
        var response = new Response<TResponse>();
        _responseMap.Add(type, response);

        return response;
    }
}
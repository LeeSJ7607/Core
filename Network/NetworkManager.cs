using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

internal sealed class NetworkManager : Singleton<NetworkManager>
{
    private interface IMessage
    {
        void Response(object message);
    }
    
    private sealed class Response<TResponse> : IMessage where TResponse : class, IResponse
    {
        public UniTask<TResponse> Task => _tcs.Task;
        private readonly UniTaskCompletionSource<TResponse> _tcs;
        private readonly Action<TResponse> _act;
        
        public Response(Action<TResponse> act = null)
        {
            _act = act;
            _tcs = act == null ? new UniTaskCompletionSource<TResponse>() : null;
        }
        
        void IMessage.Response(object message)
        {
            if (_act != null)
            {
                _act.Invoke(message as TResponse);
                return;
            }

            _tcs.TrySetResult(message as TResponse);
        }
    }
    
    private readonly Session _session = new();
    private readonly Dictionary<Type, IMessage> _responseMap = new();
    
    public UniTask<TResponse> SendAsync<TResponse>(IRequest request) where TResponse : class, IResponse
    {
        _session.Send<TResponse>(request);
        return CreateResponseTask<TResponse>().Task;
    }

    public void Send<TResponse>(IRequest request, Action<TResponse> act) where TResponse : class, IResponse
    {
        _session.Send<TResponse>(request);
        CreateResponseAction(act);
    }

    public void Receive(Type responseType, object body)
    {
        if (!_responseMap.Remove(responseType, out var old))
        {
            Debug.LogWarning($"No response handler found for type: {responseType}");
            return;
        }
        
        old.Response(body);
    }

    private void RegisterResponse<TResponse>(IMessage message) where TResponse : class, IResponse
    {
        var type = typeof(TResponse);

        if (_responseMap.Remove(type, out var old))
        {
            old.Response(null);
        }
        
        _responseMap.Add(type, message);
    }

    private Response<TResponse> CreateResponseTask<TResponse>() where TResponse : class, IResponse
    {
        var response = new Response<TResponse>();
        RegisterResponse<TResponse>(response);
        return response;
    }
    
    private void CreateResponseAction<TResponse>(Action<TResponse> act) where TResponse : class, IResponse
    {
        var response = new Response<TResponse>(act);
        RegisterResponse<TResponse>(response);
    }
}
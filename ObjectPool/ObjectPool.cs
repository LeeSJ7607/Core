using System;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IObjectPool
{
    float LifeTime { get; }
}

public sealed class ObjectPool<T> : MonoBehaviour 
    where T : MonoBehaviour, IObjectPool
{
    private sealed class PoolData
    {
        public T Obj { get; }
        private readonly float _createdTime;

        public PoolData(T obj)
        {
            Obj = obj;
            _createdTime = Time.realtimeSinceStartup;
        }

        public bool CanHide()
        {
            if (_createdTime == 0f)
            {
                return false;
            }
            
            var time = Time.realtimeSinceStartup - _createdTime;
            return time >= Obj.LifeTime;
        }
    }

    private readonly CancellationTokenSource _tokenSource = new();
    private readonly List<PoolData> _showPool = new();
    private readonly List<PoolData> _hidePool = new();
    private int _maxHidePoolCount;
    private Transform _root;

    private void OnDestroy()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
    }

    private void OnDisable()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
    }

    public void Initialize(int createCount = 4, int maxHidePoolCount = 0, Transform root = null)
    {
        _showPool.Capacity = _hidePool.Capacity = createCount;
        _maxHidePoolCount = maxHidePoolCount;
        _root = root ?? UIManager.Instance.transform;
        AddHidePool(createCount);
        OnUpdate().Forget();
    }
    
    public T Get()
    {
        var poolData = GetFromPoolData();
        poolData.Obj.Show();
        _showPool.Add(poolData);
        return poolData.Obj;
    }

    private void AddHidePool(int createCount)
    {
        for (var i = 0; i < createCount; i++)
        {
            _hidePool.Add(CreatePoolData());
        }
    }
    
    private async UniTaskVoid OnUpdate()
    {
        while (!_tokenSource.Token.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _tokenSource.Token);
            TryHidePool();
            TrimPool();
        }
    }

    private void TryHidePool()
    {
        for (var i = _showPool.Count - 1; i >= 0; i--)
        {
            var poolData = _showPool[i];
            if (!poolData.CanHide())
            {
                continue;
            }
            
            poolData.Obj.Hide();
            _showPool.RemoveAt(i);
            _hidePool.Add(poolData);
        }
    }

    private void TrimPool()
    {
        if (_maxHidePoolCount == 0)
        {
            return;
        }
        
        while (_hidePool.Count > _maxHidePoolCount)
        {
            var obj = _hidePool[0].Obj;
            Destroy(obj.gameObject);
            _hidePool.RemoveAt(0);
        }
    }

    private PoolData GetFromPoolData()
    {
        if (_hidePool.Count == 0)
        {
            return CreatePoolData();
        }
        
        var lastIdx = _hidePool.Count - 1;
        var hidePoolData = _hidePool[lastIdx];
        _hidePool.RemoveAt(lastIdx);
        return hidePoolData;
    }

    private PoolData CreatePoolData()
    {
        var res = AddressableManager.Instance.Get<T>();
        var obj = Instantiate(res, _root);
        return new PoolData(obj);
    }
}
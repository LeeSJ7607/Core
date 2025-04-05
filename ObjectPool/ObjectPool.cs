using System.Collections.Generic;
using UnityEngine;

public interface IObjectPool
{
    float LifeTime { get; }
}

public sealed class ObjectPool<T> where T : MonoBehaviour, IObjectPool
{
    private sealed class PoolData
    {
        public T Obj { get; }
        private float _createdTime;

        public PoolData(T obj)
        {
            Obj = obj;
        }

        public void Show()
        {
            _createdTime = Time.realtimeSinceStartup;
            Obj.Show();
        }

        public bool CanHide()
        {
            var lifeTime = Obj.LifeTime;
            if (lifeTime == 0f)
            {
                return false;
            }
            
            var time = Time.realtimeSinceStartup - _createdTime;
            return time >= lifeTime;
        }
    }

    private const float UPDATE_INTERVAL = 0.1f;
    private float _lastUpdateTime;
    private readonly List<PoolData> _showPool = new();
    private readonly List<PoolData> _hidePool = new();
    private int _maxHidePoolCount;
    private Transform _root;
    private string _addressKey;
    
    public void Initialize(int preloadCount = 4, int maxHidePoolCount = 0, Transform root = null)
    {
        _showPool.Capacity = _hidePool.Capacity = preloadCount;
        _maxHidePoolCount = maxHidePoolCount;
        _root = root ?? UIManager.Instance.CanvasTm;
        _addressKey = typeof(T).Name;
        _lastUpdateTime = Time.realtimeSinceStartup;
        AddHidePool(preloadCount);
    }
    
    public void OnUpdate()
    {
        if (Time.realtimeSinceStartup - _lastUpdateTime < UPDATE_INTERVAL)
        {
            return;
        }

        _lastUpdateTime = Time.realtimeSinceStartup;
        TryHidePool();
        TrimPool();
    }
    
    public T Get()
    {
        var poolData = GetFromPoolData();
        poolData.Show();
        _showPool.Add(poolData);
        return poolData.Obj;
    }

    private void AddHidePool(int createCount)
    {
        for (var i = 0; i < createCount; i++)
        {
            var poolData = CreatePoolData();
            poolData.Obj.Hide();
            _hidePool.Add(poolData);
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
            UnityEngine.Object.Destroy(obj.gameObject);
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
        var res = AddressableManager.Instance.Get<GameObject>(_addressKey);
        var obj = UnityEngine.Object.Instantiate(res, _root);
        return new PoolData(obj.GetComponent<T>());
    }
}
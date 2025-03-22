using UnityEngine;

[DisallowMultipleComponent] [RequireComponent(typeof(InGameScene))]
public sealed class Zone : MonoBehaviour
{
    private readonly BattleEnvironment _battleEnvironment = new();
    private District[] _districts;
    private int _curDistrictIdx;

    private void OnDestroy()
    {
        _battleEnvironment.Release();
    }

    private void Awake()
    {
        _districts = GetComponentsInChildren<District>(true);
        if (_districts.IsNullOrEmpty())
        {
            Debug.LogError($"{nameof(District)}가 컴포넌트 되어있지 않습니다.");
        }
    }
    
    public void Initialize()
    {
        for (var i = 0; i < _districts.Length; i++)
        {
            var district = _districts[i];
            district.Initialize(_battleEnvironment);
            district.SetActive(i == 0);
        }
    }
}
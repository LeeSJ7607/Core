using UnityEngine;

[DisallowMultipleComponent]
public sealed class Zone : MonoBehaviour
{
    private IDistrict[] _districts;
    private int _curDistrictIdx;

    private void Awake()
    {
        _districts = GetComponentsInChildren<IDistrict>();
        if (_districts.IsNullOrEmpty())
        {
            Debug.LogError($"{nameof(IDistrict)}가 컴포넌트 되어있지 않습니다.");
        }
    }

    private void Start()
    {
        _districts[_curDistrictIdx].IsActive = true;
    }
}
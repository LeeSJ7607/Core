using UnityEngine;

[DisallowMultipleComponent]
public sealed class Zone : MonoBehaviour
{
    private District[] _districts;
    private int _curDistrictIdx;

    private void Awake()
    {
        _districts = GetComponentsInChildren<District>(true);
        if (_districts.IsNullOrEmpty())
        {
            Debug.LogError($"{nameof(District)}가 컴포넌트 되어있지 않습니다.");
            return;
        }

        for (var i = 0; i < _districts.Length; i++)
        {
            _districts[i].SetActive(i == 0);
        }
    }
}
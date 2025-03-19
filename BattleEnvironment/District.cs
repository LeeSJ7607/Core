using UnityEngine;

public interface IDistrict
{
    bool IsActive { get; set; }
    bool IsCleared { get; }
}

[DisallowMultipleComponent]
public sealed class District : MonoBehaviour, IDistrict
{
    bool IDistrict.IsActive
    {
        get => _htRoot.gameObject.activeSelf;
        set => _htRoot.SetActive(value);
    }
    bool IDistrict.IsCleared => false;
    private HTRoot _htRoot;

    private void Awake()
    {
        _htRoot = GetComponentInChildren<HTRoot>(true);
        if (_htRoot == null)
        {
            Debug.LogError($"{nameof(HTRoot)}가 컴포넌트 되어있지 않습니다.");
            return;
        }
        
        _htRoot.Hide();
    }
}
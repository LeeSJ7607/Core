using UnityEngine;

public sealed class Zone : MonoBehaviour
{
    private IDistrict[] _districts;

    private void Awake()
    {
        _districts = GetComponentsInChildren<IDistrict>();
    }

    private void Start()
    {
        _districts[0].IsActive = true;
    }
}
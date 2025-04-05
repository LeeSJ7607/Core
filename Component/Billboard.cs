using UnityEngine;

internal sealed class Billboard : MonoBehaviour
{
    private Transform _tm;
    private Camera _mainCam;

    private void Awake()
    {
        _tm = transform;
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (_tm == null || _mainCam == null)
        {
            return;
        }
        
        _tm.LookAt(_tm.position + _mainCam.transform.forward);
    }
}
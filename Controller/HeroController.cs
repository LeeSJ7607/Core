using UnityEngine;

public sealed class HeroController
{
    private readonly Unit _hero;
    private readonly MoveController _moveController;
    private readonly Camera _mainCam;
    
    public HeroController(Unit hero)
    {
        _hero = hero;
        _moveController = new MoveController(hero);
        _mainCam = Camera.main ?? throw new System.NullReferenceException("Main camera is missing.");
    }

    public void OnUpdate()
    {
        UpdateCameraPos();
        HandleMovement();
    }

    private void UpdateCameraPos()
    {
        var pos = _hero.transform.position;
        // pos.y = 10f;
        // pos.z -= 5f;
        
        _mainCam.transform.position = pos;
    }

    private void HandleMovement()
    {
        if (!TryGetHitPoint(out var pos))
        {
            return;
        }
        
        _moveController.MoveTo(pos);
    }

    private bool TryGetHitPoint(out Vector3 pos)
    {
        pos = default;
        
        if (!Input.GetMouseButtonDown(0))
        {
            return false;
        }
        
        const float maxDistance = 1000f;
        var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, maxDistance))
        {
            return false;
        }

        pos = hit.point;
        pos.y = _hero.transform.position.y;
        return true;
    }
}